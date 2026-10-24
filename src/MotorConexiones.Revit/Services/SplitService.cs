using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Fabrication;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Storage;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit.Services
{
    /// <summary>Dónde se partiría una barra (el <c>dry_run</c> de <c>conn_split_member</c> y la cabecera del botón).</summary>
    public sealed class SplitPreview
    {
        public SplitPreview(long elementId, long chordElementId, SplitPointResult result, double plateThicknessMm, double gapMm, string memberType, string chordType, double shortMm, double longMm)
        {
            ElementId = elementId;
            ChordElementId = chordElementId;
            Result = result;
            PlateThicknessMm = plateThicknessMm;
            GapMm = gapMm;
            MemberType = memberType;
            ChordType = chordType;
            ShortMm = shortMm;
            LongMm = longMm;
        }

        public long ElementId { get; }
        public long ChordElementId { get; }
        public SplitPointResult Result { get; }
        public double PlateThicknessMm { get; }
        public double GapMm { get; }
        public string MemberType { get; }
        public string ChordType { get; }
        public double ShortMm { get; }
        public double LongMm { get; }

        public object ToData() => new
        {
            element_id = ElementId,
            chord_element_id = ChordElementId,
            member_type = MemberType,
            chord_type = ChordType,
            theta_deg = Math.Round(Result.ThetaDeg, 1),
            face_distance_mm = Math.Round(Result.FaceDistanceMm, 1),
            offset_from_chord_face_mm = Math.Round(Result.FirstPlateDistanceMm - Result.FaceDistanceMm, 1),
            first_plate_distance_mm = Math.Round(Result.FirstPlateDistanceMm, 1),
            plate_thickness_mm = Math.Round(PlateThicknessMm, 3),
            gap_mm = GapMm,
            split_distance_mm = Math.Round(Result.SplitDistanceMm, 1),
            split_point_mm = Result.SplitPointArray,
            cut_point_mm = new[] { Math.Round(Result.CutPointMm.X, 1), Math.Round(Result.CutPointMm.Y, 1), Math.Round(Result.CutPointMm.Z, 1) },
            short_segment_mm = Math.Round(ShortMm, 1),
            long_segment_mm = Math.Round(LongMm, 1),
        };

        /// <summary>"La diagonal 1249631 (HSS 2-1/2) llega al cordón 1245533 a 44,4°: cara a 145 mm, primera plancha a 475, corte a 485 mm por el eje (tramo corto 340, largo 3 160)".</summary>
        public string Describe() => string.Format(CultureInfo.InvariantCulture,
            "La barra {0} ({1}) llega al cordón {2} a {3:0.0}°: la cara del cordón queda a {4:0} mm por su eje, la primera plancha a {5:0} y el corte a {6:0} mm (tramo corto {7:0} mm con el id original, tramo largo {8:0} mm nuevo).",
            ElementId, MemberType, ChordElementId, Result.ThetaDeg, Result.FaceDistanceMm, Result.FirstPlateDistanceMm, Result.SplitDistanceMm, ShortMm, LongMm).Replace('.', ',');
    }

    /// <summary>
    /// Fase 13 (D5 a D8): partir una barra y deshacer el corte, en un solo sitio para <c>conn_split_member</c>, <c>conn_unsplit</c>,
    /// el botón Partir barra, la cadena de Conectar y el lote. Calcula el punto con <see cref="SplitPoint"/> (Core), comprueba
    /// <c>SPLIT_IN_USE</c> contra las conexiones del add-in, parte con <see cref="MemberSplitter"/> y guarda el
    /// <see cref="SplitRecord"/> en Extensible Storage <b>antes</b> de confirmar. Sin ventanas.
    /// </summary>
    public static class SplitService
    {
        /// <summary>El punto de corte sin tocar el modelo. Lanza <see cref="CatalogException"/> con el código que toque.</summary>
        public static SplitPreview Preview(Document document, long elementId, long chordElementId, double? offsetFromChordFaceMm = null)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            var facts = new RevitModelFacts(document);
            PanelChoices choices = PanelChoicesLoader.Load(LimitsConfigLoader.Load());
            double offset = offsetFromChordFaceMm ?? choices.DiagonalSpliceOffsetMm;
            if (offset <= 0)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "offset_from_chord_face_mm debe ser mayor que 0 (330 por defecto, Detalle B).", "offset_from_chord_face_mm",
                    "Es la distancia de la cara del cordón a la primera plancha del empalme, por el eje de la barra.");
            }
            if (elementId == chordElementId)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "La barra y el cordón son el mismo elemento (" + elementId + ").", "chord_element_id", "Pasa la diagonal en element_id y el cordón al que llega en chord_element_id.");
            }
            TemplateNode node = TemplateNode.FromModelFacts(facts, chordElementId, new[] { elementId });
            if (node.IsSplice)
            {
                throw new CatalogException(ErrorCodes.NodeAxesParallel, "La barra " + elementId + " es colineal con " + chordElementId + ": no llega a su cara, no hay dónde medir el empalme.", "element_id",
                    "El corte de la Fase 13 es para la diagonal que llega al cordón con un ángulo.");
            }
            MemberModelFacts chord = facts.GetMemberFacts(chordElementId)!;
            MemberModelFacts bar = facts.GetMemberFacts(elementId)!;
            EndPlateDetailDefaults? detail = choices.Starter?.EndPlate?.Find(bar.TypeName);
            double thickness = detail?.ThicknessMm ?? SplitPoint.DefaultPlateThicknessMm;
            double gap = Math.Max(0.0, choices.Starter?.EndPlate?.GapMm ?? 0.0);
            SplitPointResult result;
            try
            {
                result = SplitPoint.FromAxes(elementId, node.Frame, chord, bar, offset, thickness, gap);
                var (s0, s1, l0, l1) = SplitPoint.Segments(bar, result);
                return new SplitPreview(elementId, chordElementId, result, thickness, gap, bar.TypeName, chord.TypeName, s0.DistanceTo(s1), l0.DistanceTo(l1));
            }
            catch (NodeGeometryException error)
            {
                throw new CatalogException(error.Code, error.Message, "element_id", error.Hint);
            }
        }

        /// <summary>
        /// <c>SPLIT_IN_USE</c> antes de partir: una conexión del add-in que use la barra <b>en el extremo que se corta</b> (su cordón no
        /// es el de este corte) perdería su barra; la del nudo en K del propio cordón sigue valiendo porque el tramo corto conserva el id.
        /// </summary>
        public static void EnsureNotInUseForSplit(Document document, long elementId, long chordElementId)
        {
            var blocking = new List<string>();
            foreach (ConnectionRecord record in ConnectionStorageManager.ListConnections(document))
            {
                ConnectionSpec? spec = SafeSpec(record);
                if (spec == null || !Uses(spec, elementId)) continue;
                if (spec.Chord != null && spec.Chord.ElementId == chordElementId) continue; // el nudo en K de este cordón: sigue valiendo (D6)
                if (spec.Members != null && spec.Members.Any(m => m.ElementId == elementId && m.ChordElementId == chordElementId)) continue;
                blocking.Add(record.ConnectionId + " (" + record.ConnectionType + ")");
            }
            if (blocking.Count > 0)
            {
                throw new CatalogException(ErrorCodes.SplitInUse,
                    "La barra " + elementId + " tiene conexión en el extremo que se cortaría: " + string.Join(", ", blocking) + ". Partirla rompería su registro.",
                    "element_id", "Borra antes esa conexión (conn_delete) o parte la barra desde su otro extremo.");
            }
        }

        /// <summary><c>SPLIT_IN_USE</c> antes de deshacer: ninguna conexión del add-in puede usar los dos tramos (salvo las que se están borrando).</summary>
        public static void EnsureNotInUseForUnsplit(Document document, SplitRecord split, IEnumerable<string>? ignoreConnectionIds = null)
        {
            var ignore = new HashSet<string>(ignoreConnectionIds ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
            var blocking = new List<string>();
            foreach (ConnectionRecord record in ConnectionStorageManager.ListConnections(document))
            {
                if (ignore.Contains(record.ConnectionId)) continue;
                ConnectionSpec? spec = SafeSpec(record);
                if (spec == null) continue;
                if (Uses(spec, split.ElementId) || Uses(spec, split.NewElementId)) blocking.Add(record.ConnectionId + " (" + record.ConnectionType + ")");
            }
            if (blocking.Count > 0)
            {
                throw new CatalogException(ErrorCodes.SplitInUse,
                    "El corte de la barra " + split.ElementId + " (tramos " + split.ElementId + " y " + split.NewElementId + ") lo usan las conexiones " + string.Join(", ", blocking) + ": no se deshace.",
                    "element_id", "Borra antes esas conexiones (conn_delete o Borrar) y vuelve a conn_unsplit.");
            }
        }

        /// <summary>
        /// Parte la barra <b>dentro de la Transaction abierta por el llamador</b> (la cadena de Conectar, el lote y la operación suelta
        /// abren la suya) y guarda el registro. Lanza si falla; el llamador revierte.
        /// </summary>
        public static SplitRecord SplitInTransaction(Document document, SplitPreview preview, string? connectionId, string? batchId, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (preview == null) throw new ArgumentNullException(nameof(preview));
            EnsureNotInUseForSplit(document, preview.ElementId, preview.ChordElementId);
            if (SplitStorageManager.FindByElement(document, preview.ElementId) is SplitRecord existing)
            {
                throw new CatalogException(ErrorCodes.SplitInUse,
                    "La barra " + preview.ElementId + " ya está partida (corte " + existing.SplitId + ", tramos " + existing.ElementId + " y " + existing.NewElementId + ").",
                    "element_id", "Deshazlo con conn_unsplit antes de volver a partir.");
            }
            FamilyInstance member = document.GetElement(new ElementId(preview.ElementId)) as FamilyInstance
                ?? throw new CatalogException(ErrorCodes.ElementNotFound, "La barra " + preview.ElementId + " no está en el modelo.", "element_id");
            SplitRecord record = MemberSplitter.Split(document, member, preview.Result, preview.ChordElementId, warnings);
            record.ConnectionId = connectionId;
            record.BatchId = batchId;
            SplitStorageManager.Save(document, record);
            return record;
        }

        /// <summary>La operación suelta: un <see cref="OperationScope"/> propio (una entrada de deshacer) con el corte y su registro.</summary>
        public static SplitRecord Split(Document document, UIApplication? uiApplication, SplitPreview preview, string? connectionId, string? batchId, List<ApiError> warnings)
        {
            SplitRecord record;
            using (var scope = new OperationScope(document, uiApplication, "split", preview.ElementId.ToString(CultureInfo.InvariantCulture), warnings))
            {
                using (Transaction tx = scope.StartTransaction(document, "MotorConexiones: Partir barra " + preview.ElementId))
                {
                    record = SplitInTransaction(document, preview, connectionId, batchId, warnings);
                    scope.CommitOrThrow(tx);
                }
                scope.Commit();
            }
            return record;
        }

        /// <summary>Deshace un corte dentro de la Transaction del llamador (tras borrar las conexiones que lo usaban). Devuelve falso si quedó parcial.</summary>
        public static bool UnsplitInTransaction(Document document, SplitRecord record, List<ApiError> warnings, IEnumerable<string>? ignoreConnectionIds = null)
        {
            EnsureNotInUseForUnsplit(document, record, ignoreConnectionIds);
            bool complete = MemberSplitter.Unsplit(document, record, warnings);
            SplitStorageManager.Delete(document, record.SplitId);
            return complete;
        }

        /// <summary>La operación suelta de deshacer (<c>conn_unsplit</c>, el botón): un <see cref="OperationScope"/> propio.</summary>
        public static bool Unsplit(Document document, UIApplication? uiApplication, SplitRecord record, List<ApiError> warnings)
        {
            bool complete;
            using (var scope = new OperationScope(document, uiApplication, "unsplit", record.SplitId, warnings))
            {
                using (Transaction tx = scope.StartTransaction(document, "MotorConexiones: Deshacer el corte de " + record.ElementId))
                {
                    complete = UnsplitInTransaction(document, record, warnings);
                    scope.CommitOrThrow(tx);
                }
                scope.Commit();
            }
            return complete;
        }

        /// <summary>
        /// Fase 13 (D8): tras borrar una conexión, deshace los cortes que ella pidió (en orden inverso) si ninguna otra conexión usa los
        /// tramos. Dentro de la Transaction del llamador. Devuelve cuántos se deshicieron; los que no, quedan como aviso.
        /// </summary>
        public static int UndoSplitsOfConnection(Document document, string connectionId, List<ApiError> warnings)
        {
            int undone = 0;
            foreach (SplitRecord record in SplitStorageManager.ByConnection(document, connectionId).OrderByDescending(r => r.CreatedUtc, StringComparer.Ordinal))
            {
                try
                {
                    UnsplitInTransaction(document, record, warnings, new[] { connectionId });
                    undone++;
                }
                catch (CatalogException error)
                {
                    warnings.Add(new ApiError(error.Error.Code, "El corte de la barra " + record.ElementId + " pedido por la conexión " + connectionId + " se queda: " + error.Error.Message, error.Error.Path, error.Error.Hint));
                }
            }
            return undone;
        }

        public static object RecordToData(SplitRecord record) => new
        {
            split_id = record.SplitId,
            element_id = record.ElementId,
            new_element_id = record.NewElementId,
            chord_element_id = record.ChordElementId,
            split_point_mm = record.SplitPointMm.Select(v => Math.Round(v, 1)).ToArray(),
            split_distance_mm = Math.Round(record.SplitDistanceMm, 1),
            offset_from_chord_face_mm = record.OffsetFromChordFaceMm,
            original_start_mm = record.OriginalStartMm.Select(v => Math.Round(v, 1)).ToArray(),
            original_end_mm = record.OriginalEndMm.Select(v => Math.Round(v, 1)).ToArray(),
            mechanism = record.Mechanism,
            approximated_parameters = record.ApproximatedParameters,
            connection_id = record.ConnectionId,
            batch_id = record.BatchId,
            created_utc = record.CreatedUtc,
        };

        private static ConnectionSpec? SafeSpec(ConnectionRecord record)
        {
            try
            {
                return ConnectionSpec.FromJson(record.SpecJson);
            }
            catch
            {
                return null;
            }
        }

        private static bool Uses(ConnectionSpec spec, long elementId) =>
            (spec.Chord != null && spec.Chord.ElementId == elementId)
            || (spec.Members != null && spec.Members.Any(m => m.ElementId == elementId))
            || (spec.Node?.ElementIds != null && spec.Node.ElementIds.Contains(elementId));
    }
}
