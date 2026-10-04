using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Sketch;

namespace MotorConexiones.Core.Editing
{
    /// <summary>
    /// Convierte una especificación en las filas de la tabla editable de la ventana de previsualización (paso 8 del
    /// guion de la Fase 5: cordón, cartela, cada barra con su unión, pernos, soldaduras, retiros y dudas). La ventana
    /// solo muestra estas filas; los cambios vuelven por <see cref="SpecFieldEditor.Apply"/>.
    /// </summary>
    public static class SpecFieldCatalog
    {
        public static readonly IReadOnlyList<string> ChordInterfaceOptions = new[] { "through_slot", "split_top_bottom", "side_lap" };
        public static readonly IReadOnlyList<string> RoleOptions = new[] { "diagonal", "vertical", "chord" };
        public static readonly IReadOnlyList<string> AttachmentTypeOptions = new[] { "welded_slot", "bolted_knife_plate" };
        public static readonly IReadOnlyList<string> WeldTypeOptions = new[] { "fillet" };

        public static List<SpecField> Build(ConnectionSpec spec, SketchNodeInput? node = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            var rows = new List<SpecField>();

            string origen = "Origen";
            rows.Add(new SpecField("source.drawing", origen, "Plano", spec.Source?.Drawing ?? "", SpecFieldKind.Text));
            rows.Add(new SpecField("source.scale", origen, "Escala", spec.Source?.Scale ?? "", SpecFieldKind.Text));

            string cordon = "Cordón";
            rows.Add(new SpecField("chord.element_id", cordon, "ElementId", spec.Chord != null ? spec.Chord.ElementId.ToString() : "", SpecFieldKind.Integer, hint: "Id de la barra en Revit"));
            rows.Add(new SpecField("chord.profile", cordon, "Perfil", spec.Chord?.Profile ?? "", SpecFieldKind.Text, hint: "Nombre del tipo cargado en el modelo"));
            rows.Add(new SpecField("chord.continuous", cordon, "Continuo", SpecValueParser.FormatBoolean(spec.Chord?.Continuous ?? true), SpecFieldKind.Boolean));
            if (node != null)
            {
                rows.Add(new SpecField("chord.width_model", cordon, "Ancho en el modelo (mm)", SpecValueParser.FormatNumber(node.ChordWidthMm), SpecFieldKind.ReadOnly, hint: node.IsSchematic ? "Croquis esquemático: sin modelo" : "Leído del modelo"));
            }

            string cartela = "Cartela";
            GussetSpec? g = spec.Gusset;
            rows.Add(new SpecField("gusset.thickness_mm", cartela, "Espesor (mm)", SpecValueParser.FormatNumber(g?.ThicknessMm), SpecFieldKind.Number, hint: "Al cambiarlo se actualiza la etiqueta si dejaba de coincidir"));
            rows.Add(new SpecField("gusset.thickness_label", cartela, "Etiqueta del espesor", g?.ThicknessLabel ?? "", SpecFieldKind.Text, hint: "Tal como está en el plano: 3/8\", PL10"));
            rows.Add(new SpecField("gusset.width_mm", cartela, "Ancho (mm)", SpecValueParser.FormatNumber(g?.WidthMm), SpecFieldKind.Number));
            rows.Add(new SpecField("gusset.height_mm", cartela, "Alto (mm)", SpecValueParser.FormatNumber(g?.HeightMm), SpecFieldKind.Number));
            rows.Add(new SpecField("gusset.chord_interface", cartela, "Unión al cordón", g?.ChordInterface ?? "", SpecFieldKind.Choice, ChordInterfaceOptions));
            rows.Add(new SpecField("gusset.weld_to_chord.size_mm", cartela, "Soldadura al cordón (mm)", SpecValueParser.FormatNumber(g?.WeldToChord?.SizeMm), SpecFieldKind.Number, hint: "Filete"));
            rows.Add(new SpecField("gusset.weld_to_chord.all_around", cartela, "Soldadura en todo el contorno", SpecValueParser.FormatBoolean(g?.WeldToChord?.AllAround ?? true), SpecFieldKind.Boolean));

            string contorno = "Contorno de la cartela";
            if (g?.Outline?.PointsMm != null)
            {
                for (int i = 0; i < g.Outline.PointsMm.Count; i++)
                {
                    rows.Add(new SpecField("gusset.outline.points_mm[" + i + "]", contorno, "Vértice " + (i + 1) + " (x; y)", SpecValueParser.FormatPoint(g.Outline.PointsMm[i]), SpecFieldKind.PointPair, hint: "mm en el sistema local del nudo"));
                }
            }

            if (spec.Members != null)
            {
                for (int i = 0; i < spec.Members.Count; i++)
                {
                    AddMemberRows(rows, spec.Members[i], i, node?.Find(spec.Members[i].ElementId));
                }
            }

            string cotas = "Cadenas de cotas";
            if (spec.DimensionChains != null)
            {
                for (int i = 0; i < spec.DimensionChains.Count; i++)
                {
                    DimensionChain chain = spec.DimensionChains[i];
                    string name = string.IsNullOrWhiteSpace(chain.Label) ? "Cadena " + (i + 1) : chain.Label!;
                    rows.Add(new SpecField("dimension_chains[" + i + "].values_mm", cotas, name + ": valores (mm)", SpecValueParser.FormatNumberList(chain.ValuesMm), SpecFieldKind.NumberList, hint: "Separados por punto y coma"));
                    rows.Add(new SpecField("dimension_chains[" + i + "].expected_total_mm", cotas, name + ": total esperado (mm)", SpecValueParser.FormatNumber(chain.ExpectedTotalMm), SpecFieldKind.Number));
                }
            }

            string dudas = "Dudas";
            if (spec.UncertainFields != null)
            {
                for (int i = 0; i < spec.UncertainFields.Count; i++)
                {
                    UncertainField u = spec.UncertainFields[i];
                    rows.Add(new SpecField("uncertain_fields[" + i + "].path", dudas, "Duda " + (i + 1) + ": campo", u.Path ?? "", SpecFieldKind.ReadOnly, hint: u.Reason));
                    rows.Add(new SpecField("uncertain_fields[" + i + "].user_confirmed_value", dudas, "Duda " + (i + 1) + ": valor confirmado", FormatConfirmed(u.UserConfirmedValue), SpecFieldKind.Text, hint: u.Reason + " (vacío = sin confirmar)"));
                }
            }

            return rows;
        }

        private static void AddMemberRows(List<SpecField> rows, MemberSpec m, int i, SketchMemberInput? input)
        {
            string p = "members[" + i + "]";
            string group = "Barra " + (i + 1) + " · " + (m.Role ?? "barra") + " " + m.ElementId;
            rows.Add(new SpecField(p + ".element_id", group, "ElementId", m.ElementId.ToString(), SpecFieldKind.Integer));
            rows.Add(new SpecField(p + ".role", group, "Rol", m.Role ?? "", SpecFieldKind.Choice, RoleOptions));
            rows.Add(new SpecField(p + ".profile", group, "Perfil", m.Profile ?? "", SpecFieldKind.Text));
            rows.Add(new SpecField(p + ".expected_angle_deg", group, "Ángulo en el plano (°)", SpecValueParser.FormatNumber(m.ExpectedAngleDeg), SpecFieldKind.Number, hint: "Leído del dibujo; se compara con el modelo"));
            if (input != null)
            {
                rows.Add(new SpecField(p + ".angle_model", group, "Ángulo con el cordón en el modelo (°)", SpecValueParser.FormatNumber(Math.Round(input.AngleToChordDeg, 1)), SpecFieldKind.ReadOnly, hint: input.FromModel ? "Leído del modelo" : "Esquemático: sin modelo"));
                rows.Add(new SpecField(p + ".width_model", group, "Ancho del perfil en el modelo (mm)", SpecValueParser.FormatNumber(input.WidthMm), SpecFieldKind.ReadOnly));
            }
            rows.Add(new SpecField(p + ".end_setback_mm", group, "Retiro del extremo (mm)", SpecValueParser.FormatNumber(m.EndSetbackMm), SpecFieldKind.Number, hint: "Distancia del punto de trabajo al extremo real de la barra"));

            AttachmentSpec? a = m.Attachment;
            string type = a?.Type ?? "";
            rows.Add(new SpecField(p + ".attachment.type", group, "Tipo de unión", type, SpecFieldKind.Choice, AttachmentTypeOptions));
            if (a == null) return;

            if (string.Equals(type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase))
            {
                KnifePlateSpec? pl = a.Plate;
                rows.Add(new SpecField(p + ".attachment.plate.thickness_mm", group, "Placa cuchilla: espesor (mm)", SpecValueParser.FormatNumber(pl?.ThicknessMm), SpecFieldKind.Number));
                rows.Add(new SpecField(p + ".attachment.plate.thickness_label", group, "Placa cuchilla: etiqueta", pl?.ThicknessLabel ?? "", SpecFieldKind.Text));
                rows.Add(new SpecField(p + ".attachment.plate.length_mm", group, "Placa cuchilla: largo (mm)", SpecValueParser.FormatNumber(pl?.LengthMm), SpecFieldKind.Number, hint: "A lo largo de la barra"));
                rows.Add(new SpecField(p + ".attachment.plate.width_mm", group, "Placa cuchilla: ancho (mm)", SpecValueParser.FormatNumber(pl?.WidthMm), SpecFieldKind.Number));
                rows.Add(new SpecField(p + ".attachment.plate.insertion_mm", group, "Placa cuchilla: inserción en la ranura (mm)", SpecValueParser.FormatNumber(pl?.InsertionMm), SpecFieldKind.Number));
                BoltPatternSpec? b = a.Bolts;
                rows.Add(new SpecField(p + ".attachment.bolts.diameter_mm", group, "Pernos: diámetro (mm)", SpecValueParser.FormatNumber(b?.DiameterMm), SpecFieldKind.Number, hint: "Al cambiarlo se actualiza la etiqueta si dejaba de coincidir"));
                rows.Add(new SpecField(p + ".attachment.bolts.diameter_label", group, "Pernos: etiqueta del diámetro", b?.DiameterLabel ?? "", SpecFieldKind.Text));
                rows.Add(new SpecField(p + ".attachment.bolts.rows", group, "Pernos: filas (a lo largo)", SpecValueParser.FormatInteger(b?.Rows), SpecFieldKind.Integer));
                rows.Add(new SpecField(p + ".attachment.bolts.columns", group, "Pernos: columnas (transversal)", SpecValueParser.FormatInteger(b?.Columns), SpecFieldKind.Integer));
                rows.Add(new SpecField(p + ".attachment.bolts.spacing_mm", group, "Pernos: paso (mm)", SpecValueParser.FormatNumber(b?.SpacingMm), SpecFieldKind.Number));
                rows.Add(new SpecField(p + ".attachment.bolts.edge_mm", group, "Pernos: distancia al borde (mm)", SpecValueParser.FormatNumber(b?.EdgeMm), SpecFieldKind.Number));
                rows.Add(new SpecField(p + ".attachment.bolts.first_row_from_plate_end_mm", group, "Pernos: 1.ª fila desde el extremo libre (mm)", SpecValueParser.FormatNumber(b?.FirstRowFromPlateEndMm), SpecFieldKind.Number));
                rows.Add(new SpecField(p + ".attachment.weld_plate_to_member.size_mm", group, "Soldadura placa-barra (mm)", SpecValueParser.FormatNumber(a.WeldPlateToMember?.SizeMm), SpecFieldKind.Number, hint: "Filete"));
                rows.Add(new SpecField(p + ".attachment.weld_plate_to_member.all_around", group, "Soldadura placa-barra en todo el contorno", SpecValueParser.FormatBoolean(a.WeldPlateToMember?.AllAround ?? true), SpecFieldKind.Boolean));
            }
            else
            {
                rows.Add(new SpecField(p + ".attachment.slot_length_mm", group, "Largo de la ranura (mm)", SpecValueParser.FormatNumber(a.SlotLengthMm), SpecFieldKind.Number));
                rows.Add(new SpecField(p + ".attachment.weld.size_mm", group, "Soldadura (mm)", SpecValueParser.FormatNumber(a.Weld?.SizeMm), SpecFieldKind.Number, hint: "Filete"));
                rows.Add(new SpecField(p + ".attachment.weld.all_around", group, "Soldadura en todo el contorno", SpecValueParser.FormatBoolean(a.Weld?.AllAround ?? true), SpecFieldKind.Boolean));
            }
        }

        /// <summary>Texto de un <c>user_confirmed_value</c> (puede venir como JsonElement desde el archivo).</summary>
        public static string FormatConfirmed(object? value)
        {
            if (value == null) return "";
            if (value is System.Text.Json.JsonElement element)
            {
                switch (element.ValueKind)
                {
                    case System.Text.Json.JsonValueKind.Null:
                    case System.Text.Json.JsonValueKind.Undefined:
                        return "";
                    case System.Text.Json.JsonValueKind.String:
                        return element.GetString() ?? "";
                    default:
                        return element.GetRawText();
                }
            }
            if (value is double d) return SpecValueParser.FormatNumber(d);
            if (value is bool b) return SpecValueParser.FormatBoolean(b);
            return value.ToString() ?? "";
        }
    }
}
