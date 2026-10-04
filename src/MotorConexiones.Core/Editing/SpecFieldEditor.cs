using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Editing
{
    /// <summary>
    /// Aplica a la especificación el texto escrito en una fila de la tabla, por su ruta JSON. No valida reglas de
    /// ingeniería (eso lo hace <see cref="SpecValidator"/> después): solo interpreta el texto y lo coloca en el campo.
    /// Mantiene coherentes <c>thickness_label</c> y <c>diameter_label</c> cuando se edita el número y la etiqueta
    /// dejaba de coincidir (regla LABEL_VALUE_MISMATCH).
    /// </summary>
    public static class SpecFieldEditor
    {
        private static readonly Regex MemberPath = new Regex(@"^members\[(\d+)\]\.(.+)$", RegexOptions.Compiled);
        private static readonly Regex OutlinePoint = new Regex(@"^gusset\.outline\.points_mm\[(\d+)\]$", RegexOptions.Compiled);
        private static readonly Regex ChainPath = new Regex(@"^dimension_chains\[(\d+)\]\.(label|values_mm|expected_total_mm)$", RegexOptions.Compiled);
        private static readonly Regex UncertainPath = new Regex(@"^uncertain_fields\[(\d+)\]\.user_confirmed_value$", RegexOptions.Compiled);

        public static SpecEditResult Apply(ConnectionSpec spec, string path, string? text)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (string.IsNullOrWhiteSpace(path)) return SpecEditResult.Failure("Ruta vacía.");
            text = text ?? "";

            Match m = MemberPath.Match(path);
            if (m.Success) return ApplyMember(spec, int.Parse(m.Groups[1].Value), m.Groups[2].Value, text);

            m = OutlinePoint.Match(path);
            if (m.Success) return ApplyOutlinePoint(spec, int.Parse(m.Groups[1].Value), text);

            m = ChainPath.Match(path);
            if (m.Success) return ApplyChain(spec, int.Parse(m.Groups[1].Value), m.Groups[2].Value, text);

            m = UncertainPath.Match(path);
            if (m.Success) return ApplyUncertain(spec, int.Parse(m.Groups[1].Value), text);

            switch (path)
            {
                case "source.drawing":
                    spec.Source = spec.Source ?? new SourceInfo();
                    spec.Source.Drawing = NullIfEmpty(text);
                    return SpecEditResult.Success();
                case "source.scale":
                    spec.Source = spec.Source ?? new SourceInfo();
                    spec.Source.Scale = NullIfEmpty(text);
                    return SpecEditResult.Success();

                case "chord.element_id":
                    spec.Chord = spec.Chord ?? new ChordSpec();
                    if (!SpecValueParser.TryParseLong(text, out long chordId) || chordId <= 0) return Invalid("un ElementId entero positivo", text);
                    spec.Chord.ElementId = chordId;
                    return SpecEditResult.Success();
                case "chord.profile":
                    spec.Chord = spec.Chord ?? new ChordSpec();
                    spec.Chord.Profile = NullIfEmpty(text);
                    return SpecEditResult.Success();
                case "chord.continuous":
                    spec.Chord = spec.Chord ?? new ChordSpec();
                    if (!SpecValueParser.TryParseBoolean(text, out bool continuous)) return Invalid("sí o no", text);
                    spec.Chord.Continuous = continuous;
                    return SpecEditResult.Success();

                case "gusset.thickness_mm":
                {
                    spec.Gusset = spec.Gusset ?? new GussetSpec();
                    SpecEditResult r = SetNullableNumber(text, v => spec.Gusset.ThicknessMm = v, allowNull: true);
                    if (!r.Ok || !spec.Gusset.ThicknessMm.HasValue) return r;
                    return SyncThicknessLabel(spec.Gusset.ThicknessMm.Value, spec.Gusset.ThicknessLabel, l => spec.Gusset.ThicknessLabel = l, "gusset.thickness_label");
                }
                case "gusset.thickness_label":
                    spec.Gusset = spec.Gusset ?? new GussetSpec();
                    spec.Gusset.ThicknessLabel = NullIfEmpty(text);
                    return SpecEditResult.Success();
                case "gusset.width_mm":
                    spec.Gusset = spec.Gusset ?? new GussetSpec();
                    return SetNullableNumber(text, v => spec.Gusset.WidthMm = v, allowNull: true);
                case "gusset.height_mm":
                    spec.Gusset = spec.Gusset ?? new GussetSpec();
                    return SetNullableNumber(text, v => spec.Gusset.HeightMm = v, allowNull: true);
                case "gusset.chord_interface":
                    spec.Gusset = spec.Gusset ?? new GussetSpec();
                    return SetChoice(text, SpecFieldCatalog.ChordInterfaceOptions, v => spec.Gusset.ChordInterface = v, allowNull: true);
                case "gusset.weld_to_chord.size_mm":
                    spec.Gusset = spec.Gusset ?? new GussetSpec();
                    spec.Gusset.WeldToChord = spec.Gusset.WeldToChord ?? new WeldSpec();
                    return SetNullableNumber(text, v => spec.Gusset.WeldToChord.SizeMm = v, allowNull: true);
                case "gusset.weld_to_chord.all_around":
                    spec.Gusset = spec.Gusset ?? new GussetSpec();
                    spec.Gusset.WeldToChord = spec.Gusset.WeldToChord ?? new WeldSpec();
                    if (!SpecValueParser.TryParseBoolean(text, out bool allAround)) return Invalid("sí o no", text);
                    spec.Gusset.WeldToChord.AllAround = allAround;
                    return SpecEditResult.Success();
                case "gusset.weld_to_chord.type":
                    spec.Gusset = spec.Gusset ?? new GussetSpec();
                    spec.Gusset.WeldToChord = spec.Gusset.WeldToChord ?? new WeldSpec();
                    return SetChoice(text, SpecFieldCatalog.WeldTypeOptions, v => spec.Gusset.WeldToChord.Type = v ?? "fillet", allowNull: false);
            }

            return SpecEditResult.Failure("El campo '" + path + "' no se edita desde la tabla.");
        }

        private static SpecEditResult ApplyMember(ConnectionSpec spec, int index, string sub, string text)
        {
            if (spec.Members == null || index < 0 || index >= spec.Members.Count)
            {
                return SpecEditResult.Failure("No existe la barra members[" + index + "].");
            }
            MemberSpec member = spec.Members[index];

            switch (sub)
            {
                case "element_id":
                    if (!SpecValueParser.TryParseLong(text, out long id) || id <= 0) return Invalid("un ElementId entero positivo", text);
                    member.ElementId = id;
                    return SpecEditResult.Success();
                case "role":
                    return SetChoice(text, SpecFieldCatalog.RoleOptions, v => member.Role = v, allowNull: true);
                case "profile":
                    member.Profile = NullIfEmpty(text);
                    return SpecEditResult.Success();
                case "expected_angle_deg":
                    return SetNullableNumber(text, v => member.ExpectedAngleDeg = v, allowNull: true);
                case "end_setback_mm":
                    return SetNullableNumber(text, v => member.EndSetbackMm = v, allowNull: true);
            }

            if (!sub.StartsWith("attachment.", StringComparison.Ordinal))
            {
                return SpecEditResult.Failure("El campo 'members[" + index + "]." + sub + "' no se edita desde la tabla.");
            }
            member.Attachment = member.Attachment ?? new AttachmentSpec();
            AttachmentSpec a = member.Attachment;
            string field = sub.Substring("attachment.".Length);

            switch (field)
            {
                case "type":
                {
                    SpecEditResult r = SetChoice(text, SpecFieldCatalog.AttachmentTypeOptions, v => a.Type = v ?? "welded_slot", allowNull: false);
                    if (!r.Ok) return r;
                    if (string.Equals(a.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase))
                    {
                        a.Plate = a.Plate ?? new KnifePlateSpec();
                        a.Bolts = a.Bolts ?? new BoltPatternSpec();
                        a.WeldPlateToMember = a.WeldPlateToMember ?? new WeldSpec();
                        return SpecEditResult.Success("Rellena la placa cuchilla y los pernos en las filas nuevas.");
                    }
                    a.Weld = a.Weld ?? new WeldSpec();
                    return SpecEditResult.Success("Rellena el largo de la ranura y la soldadura.");
                }
                case "slot_length_mm":
                    return SetNullableNumber(text, v => a.SlotLengthMm = v, allowNull: true);
                case "weld.size_mm":
                    a.Weld = a.Weld ?? new WeldSpec();
                    return SetNullableNumber(text, v => a.Weld.SizeMm = v, allowNull: true);
                case "weld.all_around":
                {
                    a.Weld = a.Weld ?? new WeldSpec();
                    if (!SpecValueParser.TryParseBoolean(text, out bool v)) return Invalid("sí o no", text);
                    a.Weld.AllAround = v;
                    return SpecEditResult.Success();
                }
                case "weld.type":
                    a.Weld = a.Weld ?? new WeldSpec();
                    return SetChoice(text, SpecFieldCatalog.WeldTypeOptions, v => a.Weld.Type = v ?? "fillet", allowNull: false);

                case "plate.thickness_mm":
                {
                    a.Plate = a.Plate ?? new KnifePlateSpec();
                    SpecEditResult r = SetNullableNumber(text, v => a.Plate.ThicknessMm = v, allowNull: true);
                    if (!r.Ok || !a.Plate.ThicknessMm.HasValue) return r;
                    return SyncThicknessLabel(a.Plate.ThicknessMm.Value, a.Plate.ThicknessLabel, l => a.Plate.ThicknessLabel = l, "members[" + index + "].attachment.plate.thickness_label");
                }
                case "plate.thickness_label":
                    a.Plate = a.Plate ?? new KnifePlateSpec();
                    a.Plate.ThicknessLabel = NullIfEmpty(text);
                    return SpecEditResult.Success();
                case "plate.length_mm":
                    a.Plate = a.Plate ?? new KnifePlateSpec();
                    return SetNullableNumber(text, v => a.Plate.LengthMm = v, allowNull: true);
                case "plate.width_mm":
                    a.Plate = a.Plate ?? new KnifePlateSpec();
                    return SetNullableNumber(text, v => a.Plate.WidthMm = v, allowNull: true);
                case "plate.insertion_mm":
                    a.Plate = a.Plate ?? new KnifePlateSpec();
                    return SetNullableNumber(text, v => a.Plate.InsertionMm = v, allowNull: true);

                case "bolts.diameter_mm":
                {
                    a.Bolts = a.Bolts ?? new BoltPatternSpec();
                    SpecEditResult r = SetNullableNumber(text, v => a.Bolts.DiameterMm = v, allowNull: true);
                    if (!r.Ok || !a.Bolts.DiameterMm.HasValue) return r;
                    double mm = a.Bolts.DiameterMm.Value;
                    if (!string.IsNullOrWhiteSpace(a.Bolts.DiameterLabel) && !LabelParser.MatchesNumericValue(a.Bolts.DiameterLabel, mm, LabelFormatter.InchToleranceMm, out _))
                    {
                        a.Bolts.DiameterLabel = LabelFormatter.Diameter(mm);
                        return SpecEditResult.Success("members[" + index + "].attachment.bolts.diameter_label actualizado a " + a.Bolts.DiameterLabel + " para que coincida con " + SpecValueParser.FormatNumber(mm) + " mm.");
                    }
                    return SpecEditResult.Success();
                }
                case "bolts.diameter_label":
                    a.Bolts = a.Bolts ?? new BoltPatternSpec();
                    a.Bolts.DiameterLabel = NullIfEmpty(text);
                    return SpecEditResult.Success();
                case "bolts.rows":
                    a.Bolts = a.Bolts ?? new BoltPatternSpec();
                    return SetNullableInteger(text, v => a.Bolts.Rows = v);
                case "bolts.columns":
                    a.Bolts = a.Bolts ?? new BoltPatternSpec();
                    return SetNullableInteger(text, v => a.Bolts.Columns = v);
                case "bolts.spacing_mm":
                    a.Bolts = a.Bolts ?? new BoltPatternSpec();
                    return SetNullableNumber(text, v => a.Bolts.SpacingMm = v, allowNull: true);
                case "bolts.edge_mm":
                    a.Bolts = a.Bolts ?? new BoltPatternSpec();
                    return SetNullableNumber(text, v => a.Bolts.EdgeMm = v, allowNull: true);
                case "bolts.first_row_from_plate_end_mm":
                    a.Bolts = a.Bolts ?? new BoltPatternSpec();
                    return SetNullableNumber(text, v => a.Bolts.FirstRowFromPlateEndMm = v, allowNull: true);

                case "weld_plate_to_member.size_mm":
                    a.WeldPlateToMember = a.WeldPlateToMember ?? new WeldSpec();
                    return SetNullableNumber(text, v => a.WeldPlateToMember.SizeMm = v, allowNull: true);
                case "weld_plate_to_member.all_around":
                {
                    a.WeldPlateToMember = a.WeldPlateToMember ?? new WeldSpec();
                    if (!SpecValueParser.TryParseBoolean(text, out bool v)) return Invalid("sí o no", text);
                    a.WeldPlateToMember.AllAround = v;
                    return SpecEditResult.Success();
                }
                case "weld_plate_to_member.type":
                    a.WeldPlateToMember = a.WeldPlateToMember ?? new WeldSpec();
                    return SetChoice(text, SpecFieldCatalog.WeldTypeOptions, v => a.WeldPlateToMember.Type = v ?? "fillet", allowNull: false);
            }

            return SpecEditResult.Failure("El campo 'members[" + index + "]." + sub + "' no se edita desde la tabla.");
        }

        private static SpecEditResult ApplyOutlinePoint(ConnectionSpec spec, int index, string text)
        {
            List<double[]>? points = spec.Gusset?.Outline?.PointsMm;
            if (points == null || index < 0 || index >= points.Count)
            {
                return SpecEditResult.Failure("No existe el vértice " + (index + 1) + " del contorno.");
            }
            if (!SpecValueParser.TryParsePoint(text, out double x, out double y))
            {
                return Invalid("dos números separados por punto y coma, por ejemplo -175; 280", text);
            }
            points[index] = new[] { x, y };
            return SpecEditResult.Success();
        }

        private static SpecEditResult ApplyChain(ConnectionSpec spec, int index, string field, string text)
        {
            if (spec.DimensionChains == null || index < 0 || index >= spec.DimensionChains.Count)
            {
                return SpecEditResult.Failure("No existe la cadena de cotas " + (index + 1) + ".");
            }
            DimensionChain chain = spec.DimensionChains[index];
            switch (field)
            {
                case "label":
                    chain.Label = NullIfEmpty(text);
                    return SpecEditResult.Success();
                case "values_mm":
                    if (!SpecValueParser.TryParseNumberList(text, out List<double> values)) return Invalid("números separados por punto y coma, por ejemplo 75; 420; 70", text);
                    chain.ValuesMm = values;
                    return SpecEditResult.Success();
                default:
                    if (!SpecValueParser.TryParseNumber(text, out double total)) return Invalid("un número en mm", text);
                    chain.ExpectedTotalMm = total;
                    return SpecEditResult.Success();
            }
        }

        private static SpecEditResult ApplyUncertain(ConnectionSpec spec, int index, string text)
        {
            if (spec.UncertainFields == null || index < 0 || index >= spec.UncertainFields.Count)
            {
                return SpecEditResult.Failure("No existe la duda " + (index + 1) + ".");
            }
            spec.UncertainFields[index].UserConfirmedValue = NullIfEmpty(text);
            return SpecEditResult.Success();
        }

        private static SpecEditResult SyncThicknessLabel(double mm, string? currentLabel, Action<string> setLabel, string labelPath)
        {
            if (string.IsNullOrWhiteSpace(currentLabel)) return SpecEditResult.Success();
            if (LabelParser.MatchesNumericValue(currentLabel, mm, LabelFormatter.InchToleranceMm, out _)) return SpecEditResult.Success();
            string label = LabelFormatter.Thickness(mm, currentLabel);
            setLabel(label);
            return SpecEditResult.Success(labelPath + " actualizado a " + label + " para que coincida con " + SpecValueParser.FormatNumber(mm) + " mm.");
        }

        private static SpecEditResult SetNullableNumber(string text, Action<double?> set, bool allowNull)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                if (!allowNull) return SpecEditResult.Failure("Este campo no puede quedar vacío.");
                set(null);
                return SpecEditResult.Success();
            }
            if (!SpecValueParser.TryParseNumber(text, out double value)) return Invalid("un número (coma o punto decimal)", text);
            set(value);
            return SpecEditResult.Success();
        }

        private static SpecEditResult SetNullableInteger(string text, Action<int?> set)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                set(null);
                return SpecEditResult.Success();
            }
            if (!SpecValueParser.TryParseInteger(text, out int value)) return Invalid("un número entero", text);
            set(value);
            return SpecEditResult.Success();
        }

        private static SpecEditResult SetChoice(string text, IReadOnlyList<string> options, Action<string?> set, bool allowNull)
        {
            string t = (text ?? "").Trim();
            if (t.Length == 0)
            {
                if (!allowNull) return SpecEditResult.Failure("Elige uno de: " + string.Join(", ", options) + ".");
                set(null);
                return SpecEditResult.Success();
            }
            foreach (string option in options)
            {
                if (string.Equals(option, t, StringComparison.OrdinalIgnoreCase))
                {
                    set(option);
                    return SpecEditResult.Success();
                }
            }
            return SpecEditResult.Failure("'" + t + "' no es un valor admitido. Elige uno de: " + string.Join(", ", options) + ".");
        }

        private static SpecEditResult Invalid(string expected, string text)
        {
            return SpecEditResult.Failure("'" + text.Trim() + "' no se entiende: se esperaba " + expected + ".");
        }

        private static string? NullIfEmpty(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        }
    }
}
