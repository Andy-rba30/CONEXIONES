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
            string guideText = "";
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
                    guideText = File.ReadAllText(guidePath);
                }
                else
                {
                    guideText = "# Guía de MotorConexiones\n\n1. Llama a conn_ping.\n2. Llama a conn_get_node_info.\n3. Valida con conn_validate.\n4. Crea con conn_create.";
                }
            }
            catch (Exception ex)
            {
                guideText = "Error al leer guide.md: " + ex.Message;
            }

            return ApiResponse.Success(Name, new { guide_markdown = guideText }, context.Warnings);
        }
    }
}
