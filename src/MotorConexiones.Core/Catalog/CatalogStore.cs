using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>Una fila de <c>conn_catalog_list</c>: lo justo para elegir una plantilla sin volcarla entera.</summary>
    public sealed class CatalogEntry
    {
        public CatalogEntry(CatalogTemplate template, string file)
        {
            TemplateId = template.TemplateId;
            Name = template.Name;
            Description = template.Description;
            ConnectionType = template.ConnectionType;
            Tags = template.Tags.ToList();
            MembersCount = template.MemberPattern.Count;
            ChordProfile = template.ChordPattern?.Profile;
            CreatedUtc = template.CreatedUtc;
            OriginDrawing = template.Origin?.Drawing;
            OriginDocument = template.Origin?.Document;
            Pattern = template.DescribePattern();
            File = file;
        }

        public string TemplateId { get; }
        public string Name { get; }
        public string? Description { get; }
        public string ConnectionType { get; }
        public List<string> Tags { get; }
        public int MembersCount { get; }
        public string? ChordProfile { get; }
        public string CreatedUtc { get; }
        public string? OriginDrawing { get; }
        public string? OriginDocument { get; }
        public string Pattern { get; }
        public string File { get; }
    }

    /// <summary>
    /// Carpeta del catálogo: un archivo <c>&lt;template_id&gt;.json</c> por plantilla (sección 2.3 de la propuesta). Sin
    /// Revit: en el add-in la carpeta sale de <c>config/catalog.json</c>; en las pruebas, de una carpeta temporal.
    /// </summary>
    public sealed class CatalogStore
    {
        private static readonly Regex ValidId = new Regex("^[A-Za-z0-9][A-Za-z0-9._-]{0,120}$", RegexOptions.CultureInvariant);

        public CatalogStore(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder)) throw new ArgumentException("La carpeta del catálogo está vacía.", nameof(folder));
            Folder = Path.GetFullPath(folder);
        }

        public string Folder { get; }

        public static bool IsValidTemplateId(string? templateId) => !string.IsNullOrWhiteSpace(templateId) && ValidId.IsMatch(templateId!.Trim());

        public string PathFor(string templateId)
        {
            if (!IsValidTemplateId(templateId))
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "template_id '" + templateId + "' no es válido.", "template_id",
                    "Usa el template_id que devuelve conn_catalog_list (letras, números, guiones y puntos).");
            }
            return Path.Combine(Folder, templateId.Trim() + ".json");
        }

        /// <summary>Crea la carpeta si no existe; lanza <c>CATALOG_FOLDER_UNAVAILABLE</c> si no se puede.</summary>
        public void EnsureFolder()
        {
            try
            {
                Directory.CreateDirectory(Folder);
            }
            catch (Exception error)
            {
                throw new CatalogException(ErrorCodes.CatalogFolderUnavailable,
                    "No se pudo crear la carpeta del catálogo '" + Folder + "': " + error.Message, "catalog_folder",
                    "Revisa catalog_folder en config\\catalog.json o los permisos de la carpeta.");
            }
        }

        /// <summary>Plantillas legibles, ordenadas por nombre. Los archivos ilegibles se anotan como avisos <c>TEMPLATE_INVALID</c>.</summary>
        public IReadOnlyList<CatalogEntry> List(List<ApiError>? warnings = null)
        {
            var entries = new List<CatalogEntry>();
            if (!Directory.Exists(Folder)) return entries;
            foreach (string file in Directory.EnumerateFiles(Folder, "*.json").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                CatalogTemplate? template = ReadFile(file, out string? error);
                if (template == null)
                {
                    warnings?.Add(new ApiError(ErrorCodes.TemplateInvalid, "El archivo '" + Path.GetFileName(file) + "' no es una plantilla legible" + (error != null ? ": " + error : "."), null,
                        "Corrígelo o bórralo de la carpeta del catálogo."));
                    continue;
                }
                entries.Add(new CatalogEntry(template, file));
            }
            return entries.OrderBy(e => e.Name, StringComparer.OrdinalIgnoreCase).ThenBy(e => e.CreatedUtc, StringComparer.Ordinal).ToList();
        }

        /// <summary>Plantilla por <c>template_id</c> o nulo si no existe.</summary>
        public CatalogTemplate? Get(string templateId)
        {
            string path = PathFor(templateId);
            if (!File.Exists(path)) return null;
            CatalogTemplate? template = ReadFile(path, out string? error);
            if (template == null)
            {
                throw new CatalogException(ErrorCodes.TemplateInvalid, "El archivo '" + path + "' no es una plantilla legible" + (error != null ? ": " + error : "."), "template_id",
                    "Corrígelo o bórralo de la carpeta del catálogo.");
            }
            return template;
        }

        /// <summary>Plantilla por nombre (sin distinguir mayúsculas) o nulo.</summary>
        public CatalogTemplate? FindByName(string name)
        {
            string wanted = (name ?? string.Empty).Trim();
            if (wanted.Length == 0) return null;
            foreach (CatalogEntry entry in List())
            {
                if (string.Equals(entry.Name, wanted, StringComparison.OrdinalIgnoreCase)) return Get(entry.TemplateId);
            }
            return null;
        }

        /// <summary>Escribe la plantilla (UTF-8 sin BOM) y devuelve la ruta del archivo.</summary>
        public string Save(CatalogTemplate template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            EnsureFolder();
            string path = PathFor(template.TemplateId);
            File.WriteAllText(path, template.ToJson() + Environment.NewLine, new UTF8Encoding(false));
            return path;
        }

        /// <summary>Borra el archivo de la plantilla; falso si no existía.</summary>
        public bool Delete(string templateId, out string path)
        {
            path = PathFor(templateId);
            if (!File.Exists(path)) return false;
            File.Delete(path);
            return true;
        }

        /// <summary>Copia el archivo de una plantilla a otra carpeta (la compartida); devuelve la ruta de la copia.</summary>
        public static string CopyTo(string sourceFile, string targetFolder)
        {
            Directory.CreateDirectory(targetFolder);
            string target = Path.Combine(targetFolder, Path.GetFileName(sourceFile));
            File.Copy(sourceFile, target, overwrite: true);
            return target;
        }

        private static CatalogTemplate? ReadFile(string file, out string? error)
        {
            error = null;
            try
            {
                CatalogTemplate? template = CatalogTemplate.FromJson(File.ReadAllText(file));
                if (template == null) error = "no tiene template_id o spec_template";
                return template;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }
    }
}
