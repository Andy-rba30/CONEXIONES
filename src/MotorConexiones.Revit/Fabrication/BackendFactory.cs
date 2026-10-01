using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Selecciona el mejor backend de fabricación disponible:
    /// 1. Intenta Camino A (<see cref="AdvanceSteelBackend"/>).
    /// 2. Si no está disponible o falla su inicialización, recurre a Camino B (<see cref="DirectShapeBackend"/>).
    /// </summary>
    public static class BackendFactory
    {
        public static IFabricationBackend GetBackend(Document document, List<ApiError> warnings)
        {
            try
            {
                var steelBackend = new AdvanceSteelBackend(warnings);
                if (steelBackend.IsAvailable)
                {
                    return steelBackend;
                }
            }
            catch
            {
                // Silencioso; continúa con el fallback
            }

            return new DirectShapeBackend(warnings);
        }
    }
}
