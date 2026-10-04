using System;
using System.IO;
using System.Reflection;
using MotorConexiones.Core.Catalog;

namespace MotorConexiones.Revit.Catalog
{
    /// <summary>
    /// Lee <c>config\catalog.json</c> de la carpeta del add-in desplegado (como <see cref="LimitsConfigLoader"/>) y
    /// resuelve las carpetas del catálogo expandiendo <c>%LOCALAPPDATA%</c>. Si el archivo falta, valores por defecto:
    /// la carpeta del usuario junto al registro (<c>%LOCALAPPDATA%\MotorConexiones\catalogo</c>).
    /// </summary>
    internal static class CatalogConfigLoader
    {
        public static string AddinFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        public static string ConfigPath => Path.Combine(AddinFolder, "config", "catalog.json");

        public static CatalogConfig Load()
        {
            try
            {
                if (File.Exists(ConfigPath)) return CatalogConfig.LoadFromFile(ConfigPath);
            }
            catch
            {
                // Sin configuración legible se usan los valores por defecto.
            }
            return CatalogConfig.Default;
        }

        /// <summary>Carpeta del catálogo del usuario, con las variables expandidas; nunca vacía.</summary>
        public static string ResolveFolder(CatalogConfig config)
        {
            string folder = CatalogConfig.ExpandFolder(config?.CatalogFolder);
            if (string.IsNullOrWhiteSpace(folder) || folder.Contains("%"))
            {
                folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MotorConexiones", "catalogo");
            }
            return folder;
        }

        /// <summary>Carpeta compartida (copia opcional) o nulo si no está configurada.</summary>
        public static string? ResolveSharedFolder(CatalogConfig config)
        {
            string folder = CatalogConfig.ExpandFolder(config?.SharedCatalogFolder);
            return string.IsNullOrWhiteSpace(folder) ? null : folder;
        }

        public static CatalogStore OpenStore(CatalogConfig config) => new CatalogStore(ResolveFolder(config));
    }
}
