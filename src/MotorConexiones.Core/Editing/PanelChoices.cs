using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Editing
{
    /// <summary>Una opción de un desplegable con rótulo y valor en mm: <c>3/8"</c> → 9,525 → "3/8" · 9,5 mm".</summary>
    public sealed class ChoiceOption
    {
        public ChoiceOption(string label, double valueMm, bool isOther = false)
        {
            Label = label ?? string.Empty;
            ValueMm = valueMm;
            IsOther = isOther;
            Text = isOther ? PanelChoices.OtherText(valueMm) : PanelChoices.FormatOption(label ?? string.Empty, valueMm);
        }

        /// <summary>Lo que se escribe en el campo <c>*_label</c> (vacío en "Otro").</summary>
        public string Label { get; }

        /// <summary>Lo que se escribe en el campo <c>*_mm</c>.</summary>
        public double ValueMm { get; }

        /// <summary>Texto del desplegable.</summary>
        public string Text { get; }

        /// <summary>Verdadero si representa un valor del JSON que no está en la lista (no cambia nada al elegirse).</summary>
        public bool IsOther { get; }

        public override string ToString() => Text;
    }

    /// <summary>Una opción con valor de contrato y texto en español (<c>welded_slot</c> → "soldada en ranura").</summary>
    public sealed class NamedChoice
    {
        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        public override string ToString() => Text;
    }

    /// <summary>Placa cuchilla por defecto al cambiar una barra a <c>bolted_knife_plate</c> (y en el arranque).</summary>
    public sealed class KnifePlateDefaults
    {
        [JsonPropertyName("thickness_label")]
        public string ThicknessLabel { get; set; } = "PL10";

        [JsonPropertyName("length_mm")]
        public double LengthMm { get; set; } = 170;

        [JsonPropertyName("width_mm")]
        public double WidthMm { get; set; } = 140;

        [JsonPropertyName("insertion_mm")]
        public double InsertionMm { get; set; } = 80;

        [JsonPropertyName("gusset_face")]
        public string GussetFace { get; set; } = "+z";

        /// <summary>Espesor en mm del rótulo (10 para PL10).</summary>
        public double ThicknessMm => LabelParser.TryParseLabelToMm(ThicknessLabel, out double mm) ? Math.Round(mm, 3) : 10.0;
    }

    /// <summary>Pernos por defecto al cambiar una barra a <c>bolted_knife_plate</c> (y en el arranque).</summary>
    public sealed class BoltDefaults
    {
        [JsonPropertyName("diameter_label")]
        public string DiameterLabel { get; set; } = "5/8\"";

        [JsonPropertyName("rows")]
        public int Rows { get; set; } = 2;

        [JsonPropertyName("columns")]
        public int Columns { get; set; } = 2;

        [JsonPropertyName("spacing_mm")]
        public double SpacingMm { get; set; } = 60;

        [JsonPropertyName("edge_mm")]
        public double EdgeMm { get; set; } = 40;

        [JsonPropertyName("first_row_from_plate_end_mm")]
        public double FirstRowFromPlateEndMm { get; set; } = 40;

        /// <summary>Diámetro en mm del rótulo (15,875 para 5/8").</summary>
        public double DiameterMm => LabelParser.TryParseLabelToMm(DiameterLabel, out double mm) ? Math.Round(mm, 3) : Math.Round(UnitConverter.InchesToMm(0.625), 3);
    }

    /// <summary>Valores de la especificación de arranque (decisión D10) y de los bloques de unión que crea el panel.</summary>
    public sealed class StarterDefaults
    {
        [JsonPropertyName("gusset_thickness_label")]
        public string GussetThicknessLabel { get; set; } = "3/8\"";

        [JsonPropertyName("chord_interface")]
        public string ChordInterface { get; set; } = "through_slot";

        [JsonPropertyName("weld_size_mm")]
        public double WeldSizeMm { get; set; } = 5;

        [JsonPropertyName("weld_all_around")]
        public bool WeldAllAround { get; set; } = true;

        /// <summary>Cuánto sobresale la cartela rectangular más allá del final de la ranura (o la placa) de cada barra.</summary>
        [JsonPropertyName("gusset_margin_mm")]
        public double GussetMarginMm { get; set; } = 40;

        /// <summary>Cuánto asoma la cartela por el otro lado del cordón cuando todas las barras están a un lado.</summary>
        [JsonPropertyName("gusset_beyond_chord_mm")]
        public double GussetBeyondChordMm { get; set; } = 40;

        [JsonPropertyName("knife_plate")]
        public KnifePlateDefaults KnifePlate { get; set; } = new KnifePlateDefaults();

        [JsonPropertyName("bolts")]
        public BoltDefaults Bolts { get; set; } = new BoltDefaults();

        /// <summary>Largo de ranura por defecto (copiado de <c>slot_length_mm</c> del archivo).</summary>
        [JsonIgnore]
        public double SlotLengthMm { get; set; } = 150;

        /// <summary>Retiro por defecto (copiado de <c>end_setback_mm</c> del archivo).</summary>
        [JsonIgnore]
        public double EndSetbackMm { get; set; } = 60;

        /// <summary>Espesor de la cartela en mm del rótulo (9,525 para 3/8").</summary>
        public double GussetThicknessMm => LabelParser.TryParseLabelToMm(GussetThicknessLabel, out double mm) ? Math.Round(mm, 3) : Math.Round(UnitConverter.InchesToMm(0.375), 3);
    }

    /// <summary>
    /// Fase 11: las listas de los desplegables del panel por secciones y los valores por defecto del arranque, leídos de
    /// <c>config/choices.json</c> (editable sin recompilar). Convierte cada rótulo a mm con <see cref="LabelParser"/> y, al
    /// revés, encuentra la opción que corresponde a un valor del JSON (tolerancia <c>label_value_tolerance_mm</c>) o
    /// devuelve "Otro: 9,2 mm" sin cambiar nada. Sin archivo, listas embebidas: nunca un panel vacío.
    /// </summary>
    public sealed class PanelChoices
    {
        private static readonly JsonSerializerOptions ReadOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        [JsonPropertyName("schema_version")]
        public int SchemaVersion { get; set; } = 1;

        [JsonPropertyName("thickness_labels")]
        public List<string> ThicknessLabels { get; set; } = new List<string> { "1/4\"", "5/16\"", "3/8\"", "1/2\"", "5/8\"", "3/4\"", "1\"", "PL6", "PL8", "PL10", "PL12", "PL16", "PL20" };

        [JsonPropertyName("bolt_diameter_labels")]
        public List<string> BoltDiameterLabels { get; set; } = new List<string> { "1/2\"", "5/8\"", "3/4\"", "7/8\"" };

        [JsonPropertyName("weld_sizes_mm")]
        public List<double> WeldSizesMm { get; set; } = new List<double> { 3, 4, 5, 6, 8, 10 };

        [JsonPropertyName("slot_length_mm")]
        public double SlotLengthMm { get; set; } = 150;

        [JsonPropertyName("end_setback_mm")]
        public double EndSetbackMm { get; set; } = 60;

        [JsonPropertyName("attachment_types")]
        public List<NamedChoice> AttachmentTypes { get; set; } = new List<NamedChoice>
        {
            new NamedChoice { Value = "welded_slot", Text = "soldada en ranura" },
            new NamedChoice { Value = "bolted_knife_plate", Text = "placa cuchilla atornillada" },
        };

        [JsonPropertyName("chord_interfaces")]
        public List<NamedChoice> ChordInterfaces { get; set; } = new List<NamedChoice>
        {
            new NamedChoice { Value = "through_slot", Text = "atraviesa el cordón ranurado" },
            new NamedChoice { Value = "split_top_bottom", Text = "partida: una cartela arriba y otra abajo del cordón" },
            new NamedChoice { Value = "side_lap", Text = "solapada en la cara del cordón" },
        };

        [JsonPropertyName("gusset_faces")]
        public List<NamedChoice> GussetFaces { get; set; } = new List<NamedChoice>
        {
            new NamedChoice { Value = "+z", Text = "+z (cara por defecto)" },
            new NamedChoice { Value = "-z", Text = "-z (cara contraria)" },
        };

        [JsonPropertyName("roles")]
        public List<NamedChoice> Roles { get; set; } = new List<NamedChoice>
        {
            new NamedChoice { Value = "diagonal", Text = "diagonal" },
            new NamedChoice { Value = "vertical", Text = "vertical (montante)" },
        };

        [JsonPropertyName("starter")]
        public StarterDefaults Starter { get; set; } = new StarterDefaults();

        /// <summary>Diámetros de la lista que no existen en <c>limits.json</c> y se descartaron (<see cref="FilterBoltDiameters"/>).</summary>
        [JsonIgnore]
        public List<string> DiscardedBoltLabels { get; } = new List<string>();

        /// <summary>De dónde salieron las listas: la ruta del archivo o "embebidas".</summary>
        [JsonIgnore]
        public string Source { get; private set; } = "embebidas";

        public static PanelChoices Default => new PanelChoices();

        /// <summary>Lee el JSON; con texto vacío o ilegible, las listas embebidas. Las listas vacías del archivo se rellenan con las embebidas.</summary>
        public static PanelChoices LoadFromJson(string? json, string? source = null)
        {
            PanelChoices result;
            try
            {
                result = string.IsNullOrWhiteSpace(json) ? Default : JsonSerializer.Deserialize<PanelChoices>(json!, ReadOptions) ?? Default;
            }
            catch (JsonException)
            {
                result = Default;
                result.Source = "embebidas (el archivo no se pudo leer)";
                return result;
            }
            PanelChoices fallback = Default;
            if (result.ThicknessLabels == null || result.ThicknessLabels.Count == 0) result.ThicknessLabels = fallback.ThicknessLabels;
            if (result.BoltDiameterLabels == null || result.BoltDiameterLabels.Count == 0) result.BoltDiameterLabels = fallback.BoltDiameterLabels;
            if (result.WeldSizesMm == null || result.WeldSizesMm.Count == 0) result.WeldSizesMm = fallback.WeldSizesMm;
            if (result.AttachmentTypes == null || result.AttachmentTypes.Count == 0) result.AttachmentTypes = fallback.AttachmentTypes;
            if (result.ChordInterfaces == null || result.ChordInterfaces.Count == 0) result.ChordInterfaces = fallback.ChordInterfaces;
            if (result.GussetFaces == null || result.GussetFaces.Count == 0) result.GussetFaces = fallback.GussetFaces;
            if (result.Roles == null || result.Roles.Count == 0) result.Roles = fallback.Roles;
            result.Starter ??= fallback.Starter;
            result.Starter.KnifePlate ??= new KnifePlateDefaults();
            result.Starter.Bolts ??= new BoltDefaults();
            if (result.SlotLengthMm <= 0) result.SlotLengthMm = fallback.SlotLengthMm;
            if (result.EndSetbackMm < 0) result.EndSetbackMm = fallback.EndSetbackMm;
            result.Starter.SlotLengthMm = result.SlotLengthMm;
            result.Starter.EndSetbackMm = result.EndSetbackMm;
            result.Source = source ?? "texto";
            return result;
        }

        /// <summary>Lee <c>config/choices.json</c>; si falta o no se puede leer, las listas embebidas. Nunca lanza.</summary>
        public static PanelChoices LoadFromFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return Default;
            try
            {
                return LoadFromJson(File.ReadAllText(filePath), filePath);
            }
            catch
            {
                return Default;
            }
        }

        /// <summary>
        /// Deja en la lista de diámetros solo los que existen en <c>limits.json</c> (<c>bolts.edge_distance_mm</c>, con la
        /// tolerancia de rótulo); los descartados quedan en <see cref="DiscardedBoltLabels"/> para el log. Devuelve cuántos se quitaron.
        /// </summary>
        public int FilterBoltDiameters(LimitsConfig? limits)
        {
            limits ??= LimitsConfig.Default;
            DiscardedBoltLabels.Clear();
            var known = new List<double>();
            foreach (string key in limits.Bolts?.EdgeDistanceMm?.Keys ?? Enumerable.Empty<string>())
            {
                if (double.TryParse(key, NumberStyles.Float, CultureInfo.InvariantCulture, out double mm)) known.Add(mm);
            }
            if (known.Count == 0) return 0;
            var kept = new List<string>();
            foreach (string label in BoltDiameterLabels)
            {
                if (LabelParser.TryParseLabelToMm(label, out double mm) && known.Any(k => Math.Abs(k - mm) <= limits.LabelValueToleranceMm + 0.01)) kept.Add(label);
                else DiscardedBoltLabels.Add(label);
            }
            BoltDiameterLabels = kept;
            return DiscardedBoltLabels.Count;
        }

        /// <summary>Opciones de espesor (cartela y placa cuchilla), en el orden del archivo; los rótulos ilegibles se saltan.</summary>
        public List<ChoiceOption> ThicknessOptions() => ToOptions(ThicknessLabels);

        /// <summary>Opciones de diámetro de perno.</summary>
        public List<ChoiceOption> BoltDiameterOptions() => ToOptions(BoltDiameterLabels);

        /// <summary>Opciones de soldadura: el rótulo es el número en mm ("5"), el texto "5 mm".</summary>
        public List<ChoiceOption> WeldOptions() => WeldSizesMm.Where(s => s > 0).Select(s => new ChoiceOption(s.ToString("0.##", CultureInfo.InvariantCulture), s)).ToList();

        /// <summary>La opción cuyo valor coincide con <paramref name="valueMm"/> dentro de la tolerancia, o nula.</summary>
        public static ChoiceOption? FindOption(IEnumerable<ChoiceOption> options, double? valueMm, double toleranceMm = 0.05)
        {
            if (!valueMm.HasValue) return null;
            return options.FirstOrDefault(o => !o.IsOther && Math.Abs(o.ValueMm - valueMm.Value) <= toleranceMm);
        }

        /// <summary>
        /// La opción que enseña el desplegable para un valor del JSON: la de la lista si coincide, o "Otro: 9,2 mm" (que no
        /// cambia el JSON) si no; nula si el valor no existe.
        /// </summary>
        public static ChoiceOption? OptionFor(IEnumerable<ChoiceOption> options, double? valueMm, double toleranceMm = 0.05)
        {
            if (!valueMm.HasValue) return null;
            return FindOption(options, valueMm, toleranceMm) ?? new ChoiceOption(string.Empty, valueMm.Value, isOther: true);
        }

        /// <summary>Texto de un valor que no está en la lista: "Otro: 9,2 mm".</summary>
        public static string OtherText(double valueMm) => "Otro: " + Mm(valueMm) + " mm";

        /// <summary>Texto de una opción: <c>3/8" · 9,5 mm</c>, <c>PL10 · 10,0 mm</c>, <c>5 mm</c> (soldaduras).</summary>
        public static string FormatOption(string label, double valueMm)
        {
            string clean = (label ?? string.Empty).Trim();
            if (clean.Length == 0) return Mm(valueMm) + " mm";
            // Rótulo numérico (soldaduras: "5"): solo el número, sin decimales de más ("5 mm", "3,5 mm").
            if (double.TryParse(clean, NumberStyles.Float, CultureInfo.InvariantCulture, out double plain) && Math.Abs(plain - valueMm) < 1e-9)
            {
                return valueMm.ToString("0.##", CultureInfo.InvariantCulture).Replace('.', ',') + " mm";
            }
            return clean + " · " + Mm(valueMm) + " mm";
        }

        /// <summary>mm con una cifra decimal y coma: 9,525 → "9,5"; 10 → "10,0".</summary>
        public static string Mm(double valueMm) => valueMm.ToString("0.0", CultureInfo.InvariantCulture).Replace('.', ',');

        private static List<ChoiceOption> ToOptions(IEnumerable<string>? labels)
        {
            var options = new List<ChoiceOption>();
            foreach (string label in labels ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(label) || !LabelParser.TryParseLabelToMm(label, out double mm)) continue;
                options.Add(new ChoiceOption(label.Trim(), Math.Round(mm, 3)));
            }
            return options;
        }
    }
}
