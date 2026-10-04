using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.Catalog
{
    /// <summary>Todo lo que devuelve aplicar una plantilla a un nudo: casado, especificación instanciada y validación.</summary>
    public sealed class CatalogApplyResult
    {
        public CatalogApplyResult(CatalogTemplate template, TemplateNode node, InstantiationResult instantiation, ModelValidation validation)
        {
            Template = template;
            Node = node;
            Instantiation = instantiation;
            Validation = validation;
        }

        public CatalogTemplate Template { get; }
        public TemplateNode Node { get; }
        public TemplateMatch Match => Instantiation.Match;
        public InstantiationResult Instantiation { get; }
        public ModelValidation Validation { get; }
    }

    /// <summary>
    /// Puente entre Revit y el catálogo del Core: lee el nudo con <see cref="RevitModelFacts"/>, construye plantillas desde
    /// una conexión guardada o desde un JSON validado, las aplica a un nudo y valida la especificación resultante con la
    /// misma regla que <c>conn_validate</c>. Lo usan las cinco operaciones <c>catalog_*</c> y las ventanas de la cinta.
    /// </summary>
    public static class CatalogService
    {
        /// <summary>
        /// Plantilla desde un JSON. Devuelve falso (sin lanzar) si la especificación no valida contra el modelo: entonces
        /// <paramref name="validation"/> trae los errores. Lanza <see cref="CatalogException"/> por problemas del nudo o del nombre.
        /// </summary>
        public static bool TryBuildFromSpec(Document document, string rawJson, ConnectionSpec? spec, TemplateMetadata metadata, CatalogConfig config,
            out CatalogTemplate? template, out ModelValidation validation)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            template = null;
            validation = ValidationService.Validate(document, rawJson, spec);
            spec = validation.Spec;
            if (spec == null || !validation.IsValid) return false;
            if (spec.Chord == null)
            {
                throw new CatalogException(ErrorCodes.TemplateSpecInvalid, "La especificación no tiene 'chord'.", "chord");
            }
            TemplateNode node = TemplateNode.FromModelFacts(validation.Facts, spec.Chord.ElementId, spec.Members.Select(m => m.ElementId));
            metadata.DocumentTitle ??= document.Title;
            template = TemplateBuilder.Build(rawJson, spec, node, metadata, config);
            return true;
        }

        /// <summary>Plantilla desde una conexión creada por el add-in (su especificación guardada en Extensible Storage).</summary>
        public static bool TryBuildFromConnection(Document document, string connectionId, TemplateMetadata metadata, CatalogConfig config,
            out CatalogTemplate? template, out ModelValidation? validation)
        {
            template = null;
            validation = null;
            ConnectionRecord? record = ConnectionStorageManager.GetConnection(document, connectionId);
            if (record == null)
            {
                throw new CatalogException(ErrorCodes.ElementNotFound, "No se encontró ninguna conexión con ID '" + connectionId + "'.", "connection_id",
                    "Usa conn_list para ver las conexiones guardadas en el modelo.");
            }
            metadata.ConnectionId = record.ConnectionId;
            bool ok = TryBuildFromSpec(document, record.SpecJson, null, metadata, config, out template, out ModelValidation modelValidation);
            validation = modelValidation;
            return ok;
        }

        /// <summary>
        /// Guarda la plantilla en la carpeta del catálogo. Si ya hay otra con el mismo nombre (o el mismo id) hace falta
        /// <paramref name="overwrite"/>; al sobrescribir por nombre se conserva el <c>template_id</c> de la existente.
        /// </summary>
        public static string Save(CatalogStore store, CatalogTemplate template, bool overwrite, string? sharedFolder, bool copyToShared, out string? sharedFile)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (template == null) throw new ArgumentNullException(nameof(template));
            sharedFile = null;

            CatalogTemplate? sameName = store.FindByName(template.Name);
            if (sameName != null && !string.Equals(sameName.TemplateId, template.TemplateId, StringComparison.OrdinalIgnoreCase))
            {
                if (!overwrite)
                {
                    throw new CatalogException(ErrorCodes.TemplateExists,
                        "Ya existe la plantilla '" + sameName.Name + "' (" + sameName.TemplateId + ").", "name",
                        "Usa otro nombre o pasa overwrite: true para sustituirla (conserva su template_id).");
                }
                template.TemplateId = sameName.TemplateId;
            }
            else if (sameName == null && store.Get(template.TemplateId) != null && !overwrite)
            {
                throw new CatalogException(ErrorCodes.TemplateExists,
                    "Ya existe una plantilla con template_id '" + template.TemplateId + "'.", "template_id",
                    "Pasa overwrite: true para sustituirla o quita template_id para crear una nueva.");
            }

            string path = store.Save(template);
            if (copyToShared && !string.IsNullOrWhiteSpace(sharedFolder))
            {
                sharedFile = CatalogStore.CopyTo(path, sharedFolder!);
            }
            return path;
        }

        /// <summary>
        /// Aplica una plantilla a un nudo: elige el cordón (dado o el más horizontal), lee el nudo, casa la plantilla en las
        /// orientaciones permitidas, instancia la especificación y la valida. Lanza <see cref="CatalogException"/>
        /// (<c>TEMPLATE_NO_MATCH</c>, códigos del nudo); los errores de validación van en el resultado, no lanzan.
        /// </summary>
        public static CatalogApplyResult Apply(Document document, CatalogTemplate template, IReadOnlyList<long> elementIds, long? chordElementId,
            TemplateOrientation? forcedOrientation, CatalogConfig config, string? batchId = null)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (template == null) throw new ArgumentNullException(nameof(template));
            var ids = (elementIds ?? Array.Empty<long>()).Distinct().ToList();
            if (chordElementId.HasValue && !ids.Contains(chordElementId.Value)) ids.Insert(0, chordElementId.Value);
            if (ids.Count < 2)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest,
                    "Se necesitan al menos 2 barras (el cordón y una que llegue al nudo); recibidas: " + ids.Count + ".", "element_ids",
                    "Selecciona en Revit el cordón y todas las barras del nudo, o pasa sus IDs en 'element_ids'.");
            }

            var facts = new RevitModelFacts(document);
            long chord = chordElementId ?? TemplateNode.ChooseChord(facts, ids);
            TemplateNode node = TemplateNode.FromModelFacts(facts, chord, ids.Where(id => id != chord));
            TemplateMatch match = TemplateMatcher.Match(template, node, null, null, forcedOrientation);
            InstantiationResult instantiation = TemplateInstantiator.Instantiate(template, match, node, config, batchId);
            ModelValidation validation = ValidationService.Validate(document, instantiation.SpecJson, instantiation.Spec);
            return new CatalogApplyResult(template, node, instantiation, validation);
        }

        /// <summary>Carpeta de Documentos donde la ventana escribe el JSON de una plantilla aplicada ("archivo virtual").</summary>
        public static string VirtualFilePath(CatalogTemplate template, long chordElementId)
        {
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string safe = new string(template.Name.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c).ToArray()).Trim();
            if (safe.Length == 0) safe = "plantilla";
            if (safe.Length > 60) safe = safe.Substring(0, 60);
            return Path.Combine(documents, "MotorConexiones", safe + "-nudo-" + chordElementId + ".json");
        }

        public static object MatchToData(TemplateMatch match) => new
        {
            orientation = match.OrientationName,
            is_complete = match.IsComplete,
            matched_count = match.MatchedCount,
            score_deg = Math.Round(match.Score, 2),
            max_deviation_deg = Math.Round(match.MaxDeviationDeg, 2),
            assignments = match.Assignments.Select(a => new
            {
                slot = a.Slot,
                role = a.Role,
                element_id = a.ElementId,
                template_angle_deg = Math.Round(a.TemplateAngleDeg, 2),
                model_angle_deg = a.ModelAngleDeg.HasValue ? Math.Round(a.ModelAngleDeg.Value, 2) : (double?)null,
                deviation_deg = a.IsAssigned ? Math.Round(a.DeviationDeg, 2) : (double?)null,
                side = a.Member?.Side,
                model_type_name = a.Member?.TypeName,
                template_profile = a.TemplateProfile,
                profile_policy = a.ProfilePolicy,
            }).ToList(),
            unmatched_slots = match.UnmatchedSlots.ToList(),
            unassigned_members = match.UnassignedMembers.ToList(),
            description = match.Describe(),
        };

        public static object EntryToData(CatalogEntry entry) => new
        {
            template_id = entry.TemplateId,
            name = entry.Name,
            description = entry.Description,
            connection_type = entry.ConnectionType,
            tags = entry.Tags,
            members_count = entry.MembersCount,
            chord_profile = entry.ChordProfile,
            pattern = entry.Pattern,
            created_utc = entry.CreatedUtc,
            origin_drawing = entry.OriginDrawing,
            origin_document = entry.OriginDocument,
            file = entry.File,
        };

        public static object PatternToData(CatalogTemplate template) => template.MemberPattern.Select(s => new
        {
            slot = s.Slot,
            role = s.Role,
            angle_deg = s.AngleDeg,
            side = s.Side,
            profile = s.Profile,
            model_type_name = s.ModelTypeName,
            profile_policy = s.ProfilePolicy,
        }).ToList();
    }
}
