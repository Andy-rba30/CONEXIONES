using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_split_member</c> (Fase 13, D5 a D8): parte una barra de armazón en dos elementos de Revit a la distancia del Detalle B
    /// medida desde la cara del cordón por el eje de la barra. <c>element_id</c>, <c>chord_element_id</c>, <c>offset_from_chord_face_mm</c>
    /// (opcional: <c>diagonal_splice_offset_mm</c> de <c>choices.json</c>, 330) y <c>dry_run</c> (devuelve el punto sin cortar). No exige
    /// <c>validation_token</c> porque no es una especificación. El tramo corto conserva el id; el registro va a Extensible Storage.
    /// </summary>
    public sealed class SplitMemberOperation : IOperation
    {
        public string Name => "split";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            if (!TryGetId(context, "element_id", out long elementId))
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest, "Falta element_id: la barra que se parte (la diagonal).", "element_id",
                    "Pasa element_id (la barra) y chord_element_id (el cordón al que llega); offset_from_chord_face_mm es opcional (330)."));
            }
            if (!TryGetId(context, "chord_element_id", out long chordId))
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest, "Falta chord_element_id: el cordón desde cuya cara se mide la distancia.", "chord_element_id",
                    "conn_get_node_info del nudo en K dice cuál es el cordón."));
            }
            double? offset = context.TryGet("offset_from_chord_face_mm", out JsonElement offsetEl) && offsetEl.ValueKind == JsonValueKind.Number ? offsetEl.GetDouble() : (double?)null;
            bool dryRun = context.TryGet("dry_run", out JsonElement dryEl) && dryEl.ValueKind == JsonValueKind.True;
            string? connectionId = context.TryGet("connection_id", out JsonElement connEl) && connEl.ValueKind == JsonValueKind.String ? connEl.GetString() : null;
            string? batchId = context.TryGet("batch_id", out JsonElement batchEl) && batchEl.ValueKind == JsonValueKind.String ? batchEl.GetString() : null;

            SplitPreview preview;
            try
            {
                preview = SplitService.Preview(doc, elementId, chordId, offset);
                if (!dryRun) SplitService.EnsureNotInUseForSplit(doc, elementId, chordId);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
            SplitRecord? existing = SplitStorageManager.FindByElement(doc, elementId);
            if (dryRun)
            {
                return ApiResponse.Success(Name, new
                {
                    dry_run = true,
                    preview = preview.ToData(),
                    description = preview.Describe(),
                    already_split = existing == null ? null : SplitService.RecordToData(existing),
                }, context.Warnings);
            }
            if (existing != null)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.SplitInUse,
                    "La barra " + elementId + " ya está partida (corte " + existing.SplitId + ", tramos " + existing.ElementId + " y " + existing.NewElementId + ").",
                    "element_id", "Deshazlo con conn_unsplit antes de volver a partir."), context.Warnings);
            }

            SplitRecord record;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                record = SplitService.Split(doc, context.UIApplication, preview, connectionId, batchId, context.Warnings);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
            catch (Exception ex)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.SplitFailed, "Fallo al partir la barra " + elementId + ": " + ex.Message, "element_id",
                    "Se realizó rollback completo: la barra sigue entera. Mira el registro."), context.Warnings);
            }
            JsonLineLogger.Write(new { @event = "split_member", element_id = elementId, chord_element_id = chordId, new_element_id = record.NewElementId, split_id = record.SplitId, mechanism = record.Mechanism, duration_ms = stopwatch.ElapsedMilliseconds });
            return ApiResponse.Success(Name, new
            {
                split = SplitService.RecordToData(record),
                preview = preview.ToData(),
                description = preview.Describe() + " Hecho: tramo nuevo " + record.NewElementId + ".",
                next = "conn_get_node_info con [" + record.ElementId + ", " + record.NewElementId + "] da el empalme (kind splice); conn_catalog_apply o Conectar ponen el Detalle B; conn_unsplit deshace el corte.",
            }, context.Warnings);
        }

        private static bool TryGetId(OperationContext context, string name, out long id)
        {
            id = 0;
            return context.TryGet(name, out JsonElement el) && el.ValueKind == JsonValueKind.Number && el.TryGetInt64(out id) && id > 0;
        }
    }

    /// <summary>
    /// <c>conn_unsplit</c> (Fase 13, D8): deshace un corte suelto (<c>element_id</c> de cualquiera de los dos tramos, o <c>split_id</c>):
    /// borra el tramo nuevo y devuelve la curva y las extensiones a la barra original, solo si ninguna conexión usa los tramos
    /// (<c>SPLIT_IN_USE</c>). Sin parámetros, lista los cortes del modelo.
    /// </summary>
    public sealed class UnsplitOperation : IOperation
    {
        public string Name => "unsplit";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            SplitRecord? record = null;
            if (context.TryGet("split_id", out JsonElement splitEl) && splitEl.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(splitEl.GetString()))
            {
                record = SplitStorageManager.Get(doc, splitEl.GetString()!.Trim());
                if (record == null) return ApiResponse.Failure(Name, new ApiError(ErrorCodes.SplitNotFound, "No hay ningún corte con split_id '" + splitEl.GetString() + "'.", "split_id", "conn_unsplit sin parámetros lista los cortes del modelo."), context.Warnings);
            }
            else if (context.TryGet("element_id", out JsonElement el) && el.ValueKind == JsonValueKind.Number && el.TryGetInt64(out long elementId))
            {
                record = SplitStorageManager.FindByElement(doc, elementId);
                if (record == null) return ApiResponse.Failure(Name, new ApiError(ErrorCodes.SplitNotFound, "La barra " + elementId + " no está partida por el add-in (ningún corte la tiene como tramo).", "element_id", "conn_unsplit sin parámetros lista los cortes del modelo."), context.Warnings);
            }
            else
            {
                IReadOnlyList<SplitRecord> all = SplitStorageManager.List(doc);
                return ApiResponse.Success(Name, new { splits_count = all.Count, splits = all.Select(SplitService.RecordToData).ToList() }, context.Warnings);
            }

            bool complete;
            try
            {
                complete = SplitService.Unsplit(doc, context.UIApplication, record, context.Warnings);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
            catch (Exception ex)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InternalError, "Fallo al deshacer el corte: " + ex.Message, "", "Se realizó rollback completo."), context.Warnings);
            }
            JsonLineLogger.Write(new { @event = "unsplit_member", element_id = record.ElementId, new_element_id = record.NewElementId, split_id = record.SplitId, complete });
            return ApiResponse.Success(Name, new
            {
                unsplit = SplitService.RecordToData(record),
                complete,
                description = "Corte deshecho: el tramo " + record.NewElementId + " se borró y la barra " + record.ElementId + " recuperó su curva" + (complete ? "." : " (parcial: mira los avisos)."),
            }, context.Warnings);
        }
    }
}
