using System;
using System.IO;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Tests
{
    /// <summary>Localiza los archivos de docs/fixtures y config desde la carpeta de salida de las pruebas.</summary>
    internal static class Fixtures
    {
        public static string RepoPath(params string[] relative)
        {
            string current = AppDomain.CurrentDomain.BaseDirectory;
            while (!string.IsNullOrEmpty(current))
            {
                string candidate = Path.Combine(current, Path.Combine(relative));
                if (File.Exists(candidate)) return Path.GetFullPath(candidate);
                current = Path.GetDirectoryName(current)!;
            }
            throw new FileNotFoundException("No se encontró " + string.Join("/", relative) + " subiendo desde " + AppDomain.CurrentDomain.BaseDirectory);
        }

        /// <summary>Fixture del Detalle D con las dos dudas confirmadas (el que usa la Fase 5 de punta a punta).</summary>
        public static (string rawJson, ConnectionSpec spec) DetalleDConfirmado()
        {
            string json = File.ReadAllText(RepoPath("docs", "fixtures", "detalle-D-confirmado.json"));
            return (json, ConnectionSpec.FromJson(json)!);
        }
    }
}
