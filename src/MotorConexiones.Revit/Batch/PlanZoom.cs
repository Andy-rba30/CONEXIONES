using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Units;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>
    /// <b>Ver en Revit</b> sin <c>ShowElements</c> (cierre de la ronda 8c): <c>UIDocument.ShowElements</c> abre siempre un
    /// cuadro de Revit ("Revit hace zoom… Cerrar"), así que el encuadre se hace directamente con
    /// <see cref="UIView.ZoomAndCenterRectangle"/> sobre una caja alrededor del punto de trabajo del nudo, en la vista
    /// activa, y se seleccionan sus barras y su marcador. No abre ningún cuadro y no cierra la ventana del plan.
    /// </summary>
    public static class PlanZoom
    {
        /// <summary>Media anchura de la caja que se encuadra (la cartela del Detalle D mide 565 × 530 mm; con 1,2 m se ven el nudo y el arranque de sus barras).</summary>
        public const double HalfSizeMm = 1200.0;

        /// <summary>
        /// Encuadra el nudo en la vista activa y selecciona sus elementos. Devuelve el texto para la barra de estado.
        /// Lanza <see cref="InvalidOperationException"/> si la vista activa no es una vista gráfica abierta.
        /// </summary>
        public static string ShowNode(UIDocument uidoc, PlanNode node)
        {
            if (uidoc == null) throw new ArgumentNullException(nameof(uidoc));
            if (node == null) throw new ArgumentNullException(nameof(node));
            Document doc = uidoc.Document;
            List<ElementId> ids = ElementsOf(doc, node);

            View view = uidoc.ActiveView;
            UIView? uiview = uidoc.GetOpenUIViews().FirstOrDefault(v => v.ViewId == view.Id);
            if (uiview == null)
            {
                throw new InvalidOperationException("La vista activa (" + view.Name + ") no se puede encuadrar: abre una vista 3D o un alzado de la cercha y vuelve a pulsar Ver en Revit.");
            }

            XYZ center = RevitGeometry.ToFeet(new Vec3(node.WorkPointMm[0], node.WorkPointMm[1], node.WorkPointMm[2]));
            double half = UnitConverter.MmToFeet(HalfSizeMm);
            uiview.ZoomAndCenterRectangle(center - new XYZ(half, half, half), center + new XYZ(half, half, half));
            if (ids.Count > 0) uidoc.Selection.SetElementIds(ids);
            uidoc.RefreshActiveView();
            return "Nudo " + node.Name + " encuadrado en la vista " + view.Name + " (" + ids.Count + " elementos seleccionados). Orbita cuando quieras: la ventana sigue abierta.";
        }

        /// <summary>Las barras del nudo y su marcador, solo los que existan en el documento.</summary>
        public static List<ElementId> ElementsOf(Document doc, PlanNode node)
        {
            var ids = node.ElementIds.Select(id => new ElementId(id)).Where(id => doc.GetElement(id) != null).ToList();
            if (node.MarkerElementId.HasValue && doc.GetElement(new ElementId(node.MarkerElementId.Value)) != null) ids.Add(new ElementId(node.MarkerElementId.Value));
            return ids;
        }
    }
}
