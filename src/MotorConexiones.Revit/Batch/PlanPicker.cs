using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>
    /// Pinchar barras en Revit para la ventana del plan (cordón, barras que faltan, nudo nuevo). Solo admite barras de
    /// armazón estructural con eje. Se llama en contexto válido de la API (desde <see cref="PlanEvents"/>); Esc cancela y
    /// devuelve nulo o la lista vacía, sin excepción.
    /// </summary>
    public static class PlanPicker
    {
        public static Reference? PickOne(UIDocument uidoc, string prompt)
        {
            try
            {
                return uidoc.Selection.PickObject(ObjectType.Element, new FramingFilter(), prompt);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return null;
            }
        }

        public static List<long> PickMany(UIDocument uidoc, string prompt)
        {
            try
            {
                return uidoc.Selection.PickObjects(ObjectType.Element, new FramingFilter(), prompt).Select(r => r.ElementId.Value).Distinct().ToList();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return new List<long>();
            }
        }

        /// <summary>Solo barras de armazón estructural con eje.</summary>
        private sealed class FramingFilter : ISelectionFilter
        {
            private static readonly long FramingCategory = new ElementId(BuiltInCategory.OST_StructuralFraming).Value;

            public bool AllowElement(Element elem) =>
                elem is FamilyInstance fi && fi.Category != null && fi.Category.Id.Value == FramingCategory && fi.Location is LocationCurve;

            public bool AllowReference(Reference reference, XYZ position) => true;
        }
    }
}
