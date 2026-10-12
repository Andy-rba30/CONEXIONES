using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Fase 12: la plancha de una placa de extremo (<c>end_plate</c>): perpendicular al eje de la barra, con las esquinas
    /// recortadas a 45° (<c>corner_cut_mm</c>; 0 = rectangular) y soldada al tubo en todo el contorno. Los Detalles B, C e I
    /// llevan la misma plancha en los dos tramos.
    /// </summary>
    public sealed class EndPlateSpec
    {
        [JsonPropertyName("thickness_mm")]
        public double? ThicknessMm { get; set; }

        [JsonPropertyName("thickness_label")]
        public string? ThicknessLabel { get; set; }

        /// <summary>Ancho de la plancha (a lo largo de +X del marco de la plancha, el canto mayor del tubo en el Detalle C).</summary>
        [JsonPropertyName("width_mm")]
        public double? WidthMm { get; set; }

        [JsonPropertyName("height_mm")]
        public double? HeightMm { get; set; }

        /// <summary>Cateto de la esquina recortada a 45° (60 en los Detalles B, C e I); 0 o nulo = rectángulo.</summary>
        [JsonPropertyName("corner_cut_mm")]
        public double? CornerCutMm { get; set; }

        /// <summary>Soldadura de la plancha al extremo del tubo.</summary>
        [JsonPropertyName("weld")]
        public WeldSpec? Weld { get; set; }
    }

    /// <summary>Una fila de pernos en una cara de la plancha (arriba, abajo, izquierda o derecha): cuántos, a qué paso y a qué distancia del borde.</summary>
    public sealed class BoltRowSpec
    {
        [JsonPropertyName("count")]
        public int? Count { get; set; }

        /// <summary>Paso entre los pernos de la fila (irrelevante con uno solo).</summary>
        [JsonPropertyName("spacing_mm")]
        public double? SpacingMm { get; set; }

        /// <summary>Distancia del borde de la plancha al eje de la fila (25 en los Detalles B, C e I).</summary>
        [JsonPropertyName("edge_mm")]
        public double? EdgeMm { get; set; }
    }

    /// <summary>Las cuatro filas de pernos alrededor del tubo (una cara sin pernos = nula o con <c>count</c> 0).</summary>
    public sealed class BoltFacesSpec
    {
        [JsonPropertyName("top")]
        public BoltRowSpec? Top { get; set; }

        [JsonPropertyName("bottom")]
        public BoltRowSpec? Bottom { get; set; }

        [JsonPropertyName("left")]
        public BoltRowSpec? Left { get; set; }

        [JsonPropertyName("right")]
        public BoltRowSpec? Right { get; set; }
    }

    /// <summary>
    /// Fase 12: los pernos de una placa de extremo, <b>alrededor del tubo</b> por cara (no filas × columnas), con agujero,
    /// tuerca a ambos lados y longitud opcional (si falta, se calcula de la pila de dos planchas más el suplemento).
    /// </summary>
    public sealed class RingBoltSpec
    {
        [JsonPropertyName("diameter_mm")]
        public double? DiameterMm { get; set; }

        [JsonPropertyName("diameter_label")]
        public string? DiameterLabel { get; set; }

        [JsonPropertyName("hole_mm")]
        public double? HoleMm { get; set; }

        [JsonPropertyName("hole_label")]
        public string? HoleLabel { get; set; }

        /// <summary><c>both_sides</c> (los pernos atraviesan las dos planchas con tuerca a cada lado) o <c>one_side</c>.</summary>
        [JsonPropertyName("nuts")]
        public string? Nuts { get; set; }

        [JsonPropertyName("length_mm")]
        public double? LengthMm { get; set; }

        [JsonPropertyName("faces")]
        public BoltFacesSpec? Faces { get; set; }
    }

    /// <summary>
    /// Fase 12 (decisión D6): la marca del empalme Detalle B en una diagonal continua, a <c>offset_from_chord_face_mm</c> de la
    /// cara del cordón medido a lo largo de la diagonal. Se dibuja en el croquis y se escribe en el encargo; no se fabrica.
    /// </summary>
    public sealed class SpliceHintSpec
    {
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }

        [JsonPropertyName("offset_from_chord_face_mm")]
        public double? OffsetFromChordFaceMm { get; set; }
    }
}
