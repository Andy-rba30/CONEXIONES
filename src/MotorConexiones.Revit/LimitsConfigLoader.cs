using System;
using System.IO;
using System.Reflection;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Carga la configuración de límites AISC desde config/limits.json en la carpeta del add-in.
    /// Permite editar tolerancias sin recompilar el add-in.
    /// </summary>
    internal static class LimitsConfigLoader
    {
        public static LimitsConfig Load()
        {
            try
            {
                string addinFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                string limitsPath = Path.Combine(addinFolder, "config", "limits.json");
                if (File.Exists(limitsPath))
                {
                    return LimitsConfig.LoadFromFile(limitsPath);
                }
            }
            catch { }

            return LimitsConfig.Default;
        }
    }
}
