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

    /// <summary>Fase 12: valores del arranque <c>direct_weld</c> (decisión D7: filete de 5 mm en todo el contorno, sin holgura).</summary>
    public sealed class DirectWeldDefaults
    {
        [JsonPropertyName("weld_size_mm")]
        public double WeldSizeMm { get; set; } = 5;

        [JsonPropertyName("weld_all_around")]
        public bool WeldAllAround { get; set; } = true;

        [JsonPropertyName("end_gap_mm")]
        public double EndGapMm { get; set; } = 0;

        /// <summary>Verdadero = cada diagonal lleva la marca del empalme Detalle B a <c>diagonal_splice_offset_mm</c> (decisión D6).</summary>
        [JsonPropertyName("splice_hint")]
        public bool SpliceHint { get; set; } = true;

        [JsonPropertyName("splice_detail")]
        public string SpliceDetail { get; set; } = "B";
    }

    /// <summary>Una fila de pernos de un detalle de placa de extremo (por cara).</summary>
    public sealed class BoltRowDefaults
    {
        [JsonPropertyName("count")]
        public int Count { get; set; } = 1;

        [JsonPropertyName("spacing_mm")]
        public double SpacingMm { get; set; } = 0;

        [JsonPropertyName("edge_mm")]
        public double EdgeMm { get; set; } = 25;
    }

    /// <summary>Las cuatro filas de un detalle (arriba, abajo, izquierda, derecha).</summary>
    public sealed class BoltFacesDefaults
    {
        [JsonPropertyName("top")]
        public BoltRowDefaults Top { get; set; } = new BoltRowDefaults();

        [JsonPropertyName("bottom")]
        public BoltRowDefaults Bottom { get; set; } = new BoltRowDefaults();

        [JsonPropertyName("left")]
        public BoltRowDefaults Left { get; set; } = new BoltRowDefaults();

        [JsonPropertyName("right")]
        public BoltRowDefaults Right { get; set; } = new BoltRowDefaults();
    }

    /// <summary>
    /// Fase 12: un detalle de placa de extremo del plano (B, C o I) tal como lo escribe el arranque de un empalme cuyo cordón
    /// tiene ese perfil (decisión D3): plancha, esquinas, pernos por cara y agujero.
    /// </summary>
    public sealed class EndPlateDetailDefaults
    {
        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;

        [JsonPropertyName("thickness_label")]
        public string ThicknessLabel { get; set; } = "3/8\"";

        [JsonPropertyName("width_mm")]
        public double WidthMm { get; set; } = 200;

        [JsonPropertyName("height_mm")]
        public double HeightMm { get; set; } = 200;

        [JsonPropertyName("corner_cut_mm")]
        public double CornerCutMm { get; set; } = 60;

        [JsonPropertyName("diameter_label")]
        public string DiameterLabel { get; set; } = "5/8\"";

        [JsonPropertyName("hole_label")]
        public string HoleLabel { get; set; } = "Ø18";

        [JsonPropertyName("weld_size_mm")]
        public double WeldSizeMm { get; set; } = 6;

        [JsonPropertyName("faces")]
        public BoltFacesDefaults Faces { get; set; } = new BoltFacesDefaults();

        public double ThicknessMm => LabelParser.TryParseLabelToMm(ThicknessLabel, out double mm) ? Math.Round(mm, 3) : 9.525;
        public double DiameterMm => LabelParser.TryParseLabelToMm(DiameterLabel, out double mm) ? Math.Round(mm, 3) : 15.875;
        public double HoleMm => LabelParser.TryParseLabelToMm(HoleLabel, out double mm) ? Math.Round(mm, 3) : 18.0;
    }

    /// <summary>
    /// Fase 12: la regla para un perfil sin detalle propio (decisión D3): el Detalle I escalado al tubo (esquina 60, borde 25,
    /// un perno por cara a <c>canto + 2 × bolt_to_tube_mm</c>, plancha redondeada a <c>round_mm</c>).
    /// </summary>
    public sealed class EndPlateScaledDefaults
    {
        [JsonPropertyName("thickness_label")]
        public string ThicknessLabel { get; set; } = "3/8\"";

        [JsonPropertyName("diameter_label")]
        public string DiameterLabel { get; set; } = "5/8\"";

        [JsonPropertyName("hole_label")]
        public string HoleLabel { get; set; } = "Ø18";

        [JsonPropertyName("corner_cut_mm")]
        public double CornerCutMm { get; set; } = 60;

        [JsonPropertyName("edge_mm")]
        public double EdgeMm { get; set; } = 25;

        /// <summary>Distancia del eje del perno a la cara del tubo (37 en el Detalle I: 75 − 38,1).</summary>
        [JsonPropertyName("bolt_to_tube_mm")]
        public double BoltToTubeMm { get; set; } = 37;

        [JsonPropertyName("round_mm")]
        public double RoundMm { get; set; } = 5;

        [JsonPropertyName("weld_size_mm")]
        public double WeldSizeMm { get; set; } = 6;

        /// <summary>El detalle escalado a un tubo de <paramref name="tubeWidthMm"/> × <paramref name="tubeHeightMm"/> (canto mayor a lo ancho).</summary>
        public EndPlateDetailDefaults Scale(double tubeWidthMm, double tubeHeightMm)
        {
            double round = RoundMm > 0 ? RoundMm : 5.0;
            double margin = 2.0 * (BoltToTubeMm + EdgeMm);
            double width = Math.Ceiling((tubeWidthMm + margin) / round) * round;
            double height = Math.Ceiling((tubeHeightMm + margin) / round) * round;
            return new EndPlateDetailDefaults
            {
                Detail = "I escalado",
                ThicknessLabel = ThicknessLabel,
                WidthMm = width,
                HeightMm = height,
                CornerCutMm = CornerCutMm,
                DiameterLabel = DiameterLabel,
                HoleLabel = HoleLabel,
                WeldSizeMm = WeldSizeMm,
                Faces = new BoltFacesDefaults
                {
                    Top = new BoltRowDefaults { Count = 1, EdgeMm = EdgeMm },
                    Bottom = new BoltRowDefaults { Count = 1, EdgeMm = EdgeMm },
                    Left = new BoltRowDefaults { Count = 1, EdgeMm = EdgeMm },
                    Right = new BoltRowDefaults { Count = 1, EdgeMm = EdgeMm },
                },
            };
        }
    }

    /// <summary>Fase 12: valores del arranque <c>end_plate</c>: el detalle por perfil del cordón y el escalado para los demás.</summary>
    public sealed class EndPlateDefaults
    {
        [JsonPropertyName("gap_mm")]
        public double GapMm { get; set; } = 0;

        /// <summary>Perfil del cordón (rótulo AISC) → detalle del plano. Las claves se comparan con <see cref="ProfileMatcher"/>.</summary>
        [JsonPropertyName("by_profile")]
        public Dictionary<string, EndPlateDetailDefaults> ByProfile { get; set; } = DefaultDetails();

        [JsonPropertyName("default")]
        public EndPlateScaledDefaults Default { get; set; } = new EndPlateScaledDefaults();

        /// <summary>El detalle del perfil indicado (<c>HSS12X8X1/2</c> → C), o nulo si no hay ninguno para ese perfil.</summary>
        public EndPlateDetailDefaults? Find(string? profileOrTypeName)
        {
            if (string.IsNullOrWhiteSpace(profileOrTypeName) || ByProfile == null) return null;
            foreach (var pair in ByProfile)
            {
                if (ProfileMatcher.Matches(pair.Key, profileOrTypeName!)) return pair.Value;
            }
            return null;
        }

        /// <summary>El detalle del perfil, o el escalado al tubo (<paramref name="tubeWidthMm"/> × <paramref name="tubeHeightMm"/>).</summary>
        public EndPlateDetailDefaults For(string? profileOrTypeName, double tubeWidthMm, double tubeHeightMm) =>
            Find(profileOrTypeName) ?? (Default ?? new EndPlateScaledDefaults()).Scale(tubeWidthMm, tubeHeightMm);

        /// <summary>Los Detalles B, C e I del plano del Hangar (leídos en <c>docs/propuestas/placa-de-extremo.md</c>, confirmados en D2).</summary>
        public static Dictionary<string, EndPlateDetailDefaults> DefaultDetails() => new Dictionary<string, EndPlateDetailDefaults>(StringComparer.OrdinalIgnoreCase)
        {
            ["HSS12X8X1/2"] = new EndPlateDetailDefaults
            {
                Detail = "C", ThicknessLabel = "1/2\"", WidthMm = 430, HeightMm = 330, CornerCutMm = 60, DiameterLabel = "3/4\"", HoleLabel = "Ø13/16\"", WeldSizeMm = 6,
                Faces = new BoltFacesDefaults
                {
                    Top = new BoltRowDefaults { Count = 4, SpacingMm = 90, EdgeMm = 25 },
                    Bottom = new BoltRowDefaults { Count = 4, SpacingMm = 90, EdgeMm = 25 },
                    Left = new BoltRowDefaults { Count = 2, SpacingMm = 80, EdgeMm = 25 },
                    Right = new BoltRowDefaults { Count = 2, SpacingMm = 80, EdgeMm = 25 },
                },
            },
            ["HSS3X3X1/4"] = new EndPlateDetailDefaults
            {
                Detail = "I", ThicknessLabel = "3/8\"", WidthMm = 200, HeightMm = 200, CornerCutMm = 60, DiameterLabel = "5/8\"", HoleLabel = "Ø18", WeldSizeMm = 5,
                Faces = new BoltFacesDefaults
                {
                    Top = new BoltRowDefaults { Count = 1, EdgeMm = 25 }, Bottom = new BoltRowDefaults { Count = 1, EdgeMm = 25 },
                    Left = new BoltRowDefaults { Count = 1, EdgeMm = 25 }, Right = new BoltRowDefaults { Count = 1, EdgeMm = 25 },
                },
            },
            ["HSS2-1/2X2-1/2X3/16"] = new EndPlateDetailDefaults
            {
                Detail = "B", ThicknessLabel = "3/8\"", WidthMm = 185, HeightMm = 185, CornerCutMm = 60, DiameterLabel = "5/8\"", HoleLabel = "Ø18", WeldSizeMm = 5,
                Faces = new BoltFacesDefaults
                {
                    Top = new BoltRowDefaults { Count = 1, EdgeMm = 25 }, Bottom = new BoltRowDefaults { Count = 1, EdgeMm = 25 },
                    Left = new BoltRowDefaults { Count = 1, EdgeMm = 25 }, Right = new BoltRowDefaults { Count = 1, EdgeMm = 25 },
                },
            },
        };
    }

    /// <summary>Valores de la especificación de arranque (decisión D10) y de los bloques de unión que crea el panel.</summary>
    public sealed class StarterDefaults
    {
        /// <summary>Fase 12: el arranque <c>direct_weld</c> (nudo en K soldado).</summary>
        [JsonPropertyName("direct_weld")]
        public DirectWeldDefaults DirectWeld { get; set; } = new DirectWeldDefaults();

        /// <summary>Fase 12: el arranque <c>end_plate</c> (empalme) con el detalle por perfil.</summary>
        [JsonPropertyName("end_plate")]
        public EndPlateDefaults EndPlate { get; set; } = new EndPlateDefaults();

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

        /// <summary>Fase 13 (D4): del lado de la barra (o de la placa cuchilla) al borde de la cartela automática.</summary>
        [JsonPropertyName("gusset_side_margin_mm")]
        public double GussetSideMarginMm { get; set; } = 30;

        /// <summary>Fase 13: el contorno del arranque de la cartela: <c>auto</c> (calculado) o <c>polygon</c> (el rectángulo de la Fase 11).</summary>
        [JsonPropertyName("outline_mode")]
        public string OutlineMode { get; set; } = "auto";

        /// <summary>Fase 13: verdadero si el arranque escribe la cartela automática (lo de siempre desde la 0.13.0).</summary>
        [JsonIgnore]
        public bool UsesAutoOutline => !string.Equals(OutlineMode, "polygon", StringComparison.OrdinalIgnoreCase);

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

        /// <summary>Fase 12: rótulos de agujero de las placas de extremo (Ø18 para 5/8", Ø13/16" para 3/4").</summary>
        [JsonPropertyName("hole_labels")]
        public List<string> HoleLabels { get; set; } = new List<string> { "Ø18", "Ø13/16\"", "Ø15/16\"", "Ø21" };

        /// <summary>
        /// Fase 12 (decisiones P6 y D6): distancia de la cara del cordón a las planchas del empalme de la diagonal (Detalle B), medida a
        /// lo largo de la diagonal. Se dibuja, se escribe en <c>splice_hint</c> y se edita; no se fabrica en esta fase.
        /// </summary>
        [JsonPropertyName("diagonal_splice_offset_mm")]
        public double DiagonalSpliceOffsetMm { get; set; } = 330;

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
            if (result.HoleLabels == null || result.HoleLabels.Count == 0) result.HoleLabels = fallback.HoleLabels;
            if (result.DiagonalSpliceOffsetMm <= 0) result.DiagonalSpliceOffsetMm = fallback.DiagonalSpliceOffsetMm;
            result.Starter ??= fallback.Starter;
            result.Starter.KnifePlate ??= new KnifePlateDefaults();
            result.Starter.Bolts ??= new BoltDefaults();
            result.Starter.DirectWeld ??= new DirectWeldDefaults();
            result.Starter.EndPlate ??= new EndPlateDefaults();
            if (result.Starter.EndPlate.ByProfile == null || result.Starter.EndPlate.ByProfile.Count == 0) result.Starter.EndPlate.ByProfile = EndPlateDefaults.DefaultDetails();
            result.Starter.EndPlate.Default ??= new EndPlateScaledDefaults();
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

        /// <summary>Fase 12: opciones de agujero de la placa de extremo ("Ø18 · 18,0 mm").</summary>
        public List<ChoiceOption> HoleOptions() => ToOptions(HoleLabels);

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
