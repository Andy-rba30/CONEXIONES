using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>
    /// Marcas del plan en el modelo (sección 3.4 de la propuesta, opción A): en la vista activa, las barras de cada nudo
    /// se colorean con <see cref="OverrideGraphicSettings"/> (línea y superficie del color del nudo; el cordón con línea
    /// gruesa) y en el punto de trabajo se coloca un marcador <see cref="DirectShape"/> pequeño (Modelos genéricos) con
    /// el nombre del nudo: cubo para la orientación <c>same</c>, octaedro (rombo) para las orientaciones en espejo (P6).
    /// El marcador lleva <c>ApplicationId</c> = <see cref="ApplicationId"/>, <c>ApplicationDataId</c> = plan y nudo, y en
    /// Comentarios el nombre del nudo, el plan, la vista y los IDs coloreados, para poder limpiar aunque el add-in haya
    /// olvidado el plan. No escribe el parámetro <c>Marca</c> (ronda 8b: Revit avisaba "Elements have duplicate Mark values"
    /// al repetirse N1, N2… en cada plan); el nombre se ve en <c>Name</c> y en Comentarios.
    /// Siempre se llama dentro de una <c>Transaction</c> abierta por el llamador.
    /// </summary>
    public static class PlanMarks
    {
        public const string ApplicationId = "MotorConexiones.Plan";
        public const string CommentPrefix = "MotorConexiones plan ";

        /// <summary>Comentarios del marcador: "N7 · MotorConexiones plan &lt;id&gt;; view=&lt;id&gt;; ids=1,2,3; ready same; rojo".</summary>
        public static string CommentsFor(BatchPlan plan, PlanNode node, long viewId) =>
            node.Name + " · " + CommentPrefix + plan.PlanId + "; view=" + viewId.ToString(CultureInfo.InvariantCulture)
            + "; ids=" + string.Join(",", node.ElementIds.Select(i => i.ToString(CultureInfo.InvariantCulture)))
            + "; " + node.Status + (node.Orientation != null ? " " + node.Orientation : "") + "; " + node.ColorName;

        /// <summary>Lado del cubo del marcador.</summary>
        public const double MarkerSizeMm = 160.0;

        /// <summary>Aviso cuando la vista activa no admite overrides (plantilla de vista, plano...).</summary>
        public const string MarksSkipped = ErrorCodes.PlanMarksSkipped;

        /// <summary>Pone las marcas de un plan en la vista activa y anota en el plan qué se marcó.</summary>
        public static void Apply(Document document, BatchPlan plan, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            View? view = document.ActiveView;
            if (view == null || view.IsTemplate || !view.AreGraphicsOverridesAllowed())
            {
                warnings.Add(new ApiError(MarksSkipped,
                    "La vista activa no admite colores por elemento (" + (view?.Name ?? "sin vista") + "): el plan se calculó sin marcas.",
                    hint: "Abre una vista 3D o de estructura y vuelve a planificar para ver los colores."));
                plan.IsMarked = false;
                return;
            }

            ElementId solidFill = FindSolidFillPattern(document);
            ElementId categoryId = new ElementId(BuiltInCategory.OST_GenericModel);
            var marked = new List<long>();
            var markers = new List<long>();

            foreach (PlanNode node in plan.Nodes)
            {
                if (!node.CanBeMarked) continue;
                var color = new Color((byte)node.ColorRgb[0], (byte)node.ColorRgb[1], (byte)node.ColorRgb[2]);
                foreach (long id in node.ElementIds)
                {
                    Element? element = document.GetElement(new ElementId(id));
                    if (element == null || !element.IsValidObject) continue;
                    try
                    {
                        bool isChord = id == node.ChordElementId;
                        view.SetElementOverrides(element.Id, Settings(color, solidFill, isChord ? 10 : 6));
                        if (!marked.Contains(id)) marked.Add(id);
                    }
                    catch (Exception error)
                    {
                        warnings.Add(new ApiError(ErrorCodes.RevitWarning, "No se pudo colorear la barra " + id + " del nudo " + node.Name + ": " + error.Message));
                    }
                }

                try
                {
                    DirectShape marker = CreateMarker(document, categoryId, plan, node, view.Id.Value);
                    view.SetElementOverrides(marker.Id, Settings(color, solidFill, 4));
                    node.MarkerElementId = marker.Id.Value;
                    markers.Add(marker.Id.Value);
                    node.IsMarked = true;
                }
                catch (Exception error)
                {
                    warnings.Add(new ApiError(ErrorCodes.RevitWarning, "No se pudo crear el marcador del nudo " + node.Name + ": " + error.Message));
                    node.IsMarked = marked.Count > 0;
                }
            }

            plan.IsMarked = marked.Count > 0 || markers.Count > 0;
            plan.MarkedViewId = view.Id.Value;
            plan.MarkedElementIds = marked;
            plan.MarkerElementIds = markers;
        }

        /// <summary>Quita las marcas de un plan: restaura los overrides de sus barras en la vista marcada y borra sus marcadores.</summary>
        public static int Remove(Document document, BatchPlan plan, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            int removed = 0;
            View? view = plan.MarkedViewId.HasValue ? document.GetElement(new ElementId(plan.MarkedViewId.Value)) as View : null;
            if (view != null && view.IsValidObject && !view.IsTemplate)
            {
                foreach (long id in plan.MarkedElementIds)
                {
                    removed += ResetOverride(document, view, id, warnings) ? 1 : 0;
                }
            }
            foreach (long id in plan.MarkerElementIds)
            {
                removed += DeleteMarker(document, id, warnings) ? 1 : 0;
            }
            // Marcadores del plan que el registro no conozca (otra sesión del add-in): por ApplicationDataId.
            foreach (DirectShape marker in FindMarkers(document).Where(m => (m.ApplicationDataId ?? "").StartsWith(plan.PlanId + ":", StringComparison.OrdinalIgnoreCase)).ToList())
            {
                removed += RemoveByMarker(document, marker, warnings);
            }
            plan.IsMarked = false;
            plan.MarkedElementIds = new List<long>();
            plan.MarkerElementIds = new List<long>();
            foreach (PlanNode node in plan.Nodes)
            {
                node.IsMarked = false;
                node.MarkerElementId = null;
            }
            return removed;
        }

        /// <summary>
        /// Quita todas las marcas de MotorConexiones del documento, conozca o no el add-in sus planes: lee en cada
        /// marcador la vista y los IDs coloreados. Devuelve cuántos marcadores había.
        /// </summary>
        public static int RemoveAll(Document document, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            int count = 0;
            foreach (DirectShape marker in FindMarkers(document).ToList())
            {
                RemoveByMarker(document, marker, warnings);
                count++;
            }
            return count;
        }

        /// <summary>Marcadores de plan que hay en el documento.</summary>
        public static List<DirectShape> FindMarkers(Document document)
        {
            var markers = new List<DirectShape>();
            foreach (Element element in new FilteredElementCollector(document).OfClass(typeof(DirectShape)))
            {
                if (element is DirectShape shape && string.Equals(shape.ApplicationId, ApplicationId, StringComparison.Ordinal)) markers.Add(shape);
            }
            return markers;
        }

        /// <summary>IDs de todos los marcadores de plan del documento (para sondeos y para <c>batch_plan_discard</c>).</summary>
        public static List<long> MarkerIds(Document document) => FindMarkers(document).Select(m => m.Id.Value).ToList();

        private static int RemoveByMarker(Document document, DirectShape marker, List<ApiError> warnings)
        {
            int removed = 0;
            string comments = ReadComments(marker);
            // "N7 · MotorConexiones plan <id>; view=<id>; ids=1,2,3; ..." (hasta la 0.8.0: "MotorConexiones plan <id> N7; view=...")
            long viewId = 0;
            var ids = new List<long>();
            foreach (string part in comments.Split(';'))
            {
                string text = part.Trim();
                if (text.StartsWith("view=", StringComparison.OrdinalIgnoreCase)) long.TryParse(text.Substring(5), NumberStyles.Integer, CultureInfo.InvariantCulture, out viewId);
                if (text.StartsWith("ids=", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string token in text.Substring(4).Split(','))
                    {
                        if (long.TryParse(token.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long id)) ids.Add(id);
                    }
                }
            }
            if (viewId != 0 && document.GetElement(new ElementId(viewId)) is View view && view.IsValidObject && !view.IsTemplate)
            {
                foreach (long id in ids) removed += ResetOverride(document, view, id, warnings) ? 1 : 0;
            }
            removed += DeleteMarker(document, marker.Id.Value, warnings) ? 1 : 0;
            return removed;
        }

        private static bool ResetOverride(Document document, View view, long id, List<ApiError> warnings)
        {
            try
            {
                Element? element = document.GetElement(new ElementId(id));
                if (element == null || !element.IsValidObject) return false;
                view.SetElementOverrides(element.Id, new OverrideGraphicSettings());
                return true;
            }
            catch (Exception error)
            {
                warnings.Add(new ApiError(ErrorCodes.RevitWarning, "No se pudo restaurar el color de la barra " + id + ": " + error.Message));
                return false;
            }
        }

        private static bool DeleteMarker(Document document, long id, List<ApiError> warnings)
        {
            try
            {
                Element? element = document.GetElement(new ElementId(id));
                if (element == null || !element.IsValidObject) return false;
                if (element is not DirectShape shape || !string.Equals(shape.ApplicationId, ApplicationId, StringComparison.Ordinal))
                {
                    warnings.Add(new ApiError(ErrorCodes.RevitWarning, "El elemento " + id + " no es un marcador de plan: no se borra."));
                    return false;
                }
                document.Delete(element.Id);
                return true;
            }
            catch (Exception error)
            {
                warnings.Add(new ApiError(ErrorCodes.RevitWarning, "No se pudo borrar el marcador " + id + ": " + error.Message));
                return false;
            }
        }

        private static OverrideGraphicSettings Settings(Color color, ElementId solidFill, int lineWeight)
        {
            var settings = new OverrideGraphicSettings();
            settings.SetProjectionLineColor(color);
            settings.SetProjectionLineWeight(lineWeight);
            settings.SetCutLineColor(color);
            settings.SetCutLineWeight(lineWeight);
            if (solidFill != ElementId.InvalidElementId)
            {
                settings.SetSurfaceForegroundPatternVisible(true);
                settings.SetSurfaceForegroundPatternId(solidFill);
                settings.SetSurfaceForegroundPatternColor(color);
                settings.SetCutForegroundPatternVisible(true);
                settings.SetCutForegroundPatternId(solidFill);
                settings.SetCutForegroundPatternColor(color);
            }
            return settings;
        }

        private static ElementId FindSolidFillPattern(Document document)
        {
            try
            {
                foreach (Element element in new FilteredElementCollector(document).OfClass(typeof(FillPatternElement)))
                {
                    if (element is FillPatternElement pattern && pattern.GetFillPattern().IsSolidFill) return pattern.Id;
                }
            }
            catch
            {
                // Sin patrón sólido solo se colorean las líneas.
            }
            return ElementId.InvalidElementId;
        }

        private static DirectShape CreateMarker(Document document, ElementId categoryId, BatchPlan plan, PlanNode node, long viewId)
        {
            XYZ center = RevitGeometry.ToFeet(new Vec3(node.WorkPointMm[0], node.WorkPointMm[1], node.WorkPointMm[2]));
            double half = UnitConverter.MmToFeet(MarkerSizeMm / 2.0);
            Solid solid = node.IsMirrored ? Octahedron(center, half * 1.3) : Cube(center, half);
            DirectShape shape = DirectShape.CreateElement(document, categoryId);
            shape.ApplicationId = ApplicationId;
            shape.ApplicationDataId = plan.PlanId + ":" + node.Name;
            shape.SetShape(new List<GeometryObject> { solid });
            shape.Name = node.Name;
            // Sin Marca: Revit comprueba que no se repita en la categoría y avisaba al replanificar ("duplicate Mark values").
            SetText(shape, BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS, CommentsFor(plan, node, viewId));
            return shape;
        }

        private static void SetText(Element element, BuiltInParameter parameter, string value)
        {
            try
            {
                Parameter? p = element.get_Parameter(parameter);
                if (p != null && !p.IsReadOnly && p.StorageType == StorageType.String) p.Set(value);
            }
            catch
            {
                // El marcador sigue siendo útil sin el texto.
            }
        }

        private static string ReadComments(Element element)
        {
            try
            {
                return element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)?.AsString() ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static Solid Cube(XYZ center, double half)
        {
            XYZ b0 = center + new XYZ(-half, -half, -half);
            XYZ b1 = center + new XYZ(half, -half, -half);
            XYZ b2 = center + new XYZ(half, half, -half);
            XYZ b3 = center + new XYZ(-half, half, -half);
            var loop = CurveLoop.Create(new List<Curve>
            {
                Line.CreateBound(b0, b1), Line.CreateBound(b1, b2), Line.CreateBound(b2, b3), Line.CreateBound(b3, b0),
            });
            return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { loop }, XYZ.BasisZ, 2.0 * half);
        }

        /// <summary>Octaedro (rombo visto de lado): dos pirámides unidas por la base, construidas con TessellatedShapeBuilder.</summary>
        private static Solid Octahedron(XYZ center, double half)
        {
            XYZ top = center + new XYZ(0, 0, half);
            XYZ bottom = center - new XYZ(0, 0, half);
            XYZ[] ring =
            {
                center + new XYZ(half, 0, 0), center + new XYZ(0, half, 0), center - new XYZ(half, 0, 0), center - new XYZ(0, half, 0),
            };
            var builder = new TessellatedShapeBuilder();
            builder.OpenConnectedFaceSet(true);
            for (int i = 0; i < 4; i++)
            {
                XYZ a = ring[i];
                XYZ b = ring[(i + 1) % 4];
                builder.AddFace(new TessellatedFace(new List<XYZ> { a, b, top }, ElementId.InvalidElementId));
                builder.AddFace(new TessellatedFace(new List<XYZ> { b, a, bottom }, ElementId.InvalidElementId));
            }
            builder.CloseConnectedFaceSet();
            builder.Target = TessellatedShapeBuilderTarget.Solid;
            builder.Fallback = TessellatedShapeBuilderFallback.Abort;
            builder.Build();
            TessellatedShapeBuilderResult result = builder.GetBuildResult();
            foreach (GeometryObject geometry in result.GetGeometricalObjects())
            {
                if (geometry is Solid solid) return solid;
            }
            // Si la teselación no dio un sólido, el cubo sirve igual (el estado en espejo sigue en la tabla y en Comentarios).
            return Cube(center, half / 1.3);
        }
    }
}
