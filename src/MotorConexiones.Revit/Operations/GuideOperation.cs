using System;
using System.IO;
using System.Reflection;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Revit.Operations
{
    /// <summary><c>conn_get_guide</c>: devuelve el contenido de <c>docs/guide.md</c>.</summary>
    public sealed class GuideOperation : IOperation
    {
        public string Name => "guide";
        public bool RequiresDocument => false;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            return ApiResponse.Success(Name, new { guide_markdown = ReadGuideMarkdown() }, context.Warnings);
        }

        /// <summary>El texto de <c>docs/guide.md</c> desplegado junto al add-in (lo usan <c>conn_get_guide</c> y, en la Fase 10, el encargo para IA). Nunca lanza.</summary>
        public static string ReadGuideMarkdown()
        {
            try
            {
                string addinFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                string guidePath = Path.Combine(addinFolder, "docs", "guide.md");

                if (!File.Exists(guidePath))
                {
                    // Intentar relativo a la raíz del repositorio
                    guidePath = Path.Combine(addinFolder, "..", "..", "..", "docs", "guide.md");
                }

                if (File.Exists(guidePath))
                {
                    return File.ReadAllText(guidePath);
                }
                return "# Guía de MotorConexiones\n\n1. Llama a conn_ping.\n2. Llama a conn_get_node_info.\n3. Valida con conn_validate.\n4. Crea con conn_create.";
            }
            catch (Exception ex)
            {
                return "Error al leer guide.md: " + ex.Message;
            }
        }
    }
}
