using System;
using System.IO;
using System.Text;
using System.Text.Json;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Revit.Logging
{
    /// <summary>
    /// Registro en JSON por línea en <c>%LOCALAPPDATA%\MotorConexiones\log\motorconexiones-AAAAMMDD.jsonl</c>.
    /// Nunca lanza: un fallo al escribir el registro no puede tumbar una operación.
    /// </summary>
    public static class JsonLineLogger
    {
        private static readonly object Gate = new object();

        public static string Folder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MotorConexiones", "log");

        public static string CurrentFile =>
            Path.Combine(Folder, "motorconexiones-" + DateTime.Now.ToString("yyyyMMdd") + ".jsonl");

        public static void Write(object record)
        {
            try
            {
                string line = JsonSerializer.Serialize(new { ts = DateTime.Now.ToString("o"), record }, JsonOptions.Default);
                lock (Gate)
                {
                    Directory.CreateDirectory(Folder);
                    File.AppendAllText(CurrentFile, line + Environment.NewLine, new UTF8Encoding(false));
                }
            }
            catch
            {
                // Sin registro no hay operación fallida.
            }
        }
    }
}
