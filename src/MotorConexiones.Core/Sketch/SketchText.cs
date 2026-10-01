using System.Globalization;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>Textos del croquis: milímetros con una cifra decimal y coma decimal (es lo que lee la persona).</summary>
    public static class SketchText
    {
        /// <summary>565 → "565,0"; 9.525 → "9,5". No es una conversión de unidades: el croquis ya está en mm.</summary>
        public static string Mm(double valueMm)
        {
            return valueMm.ToString("0.0", CultureInfo.InvariantCulture).Replace('.', ',');
        }

        /// <summary>Grados con una cifra decimal: 45 → "45,0°".</summary>
        public static string Degrees(double degrees)
        {
            return degrees.ToString("0.0", CultureInfo.InvariantCulture).Replace('.', ',') + "°";
        }

        /// <summary>Etiqueta de espesor de una placa: <c>PL 3/8" · 9,5 mm</c> o, sin rótulo, <c>PL 10,0 mm</c>.</summary>
        public static string Thickness(string? label, double? thicknessMm)
        {
            string mm = thicknessMm.HasValue ? Mm(thicknessMm.Value) + " mm" : "espesor sin definir";
            if (string.IsNullOrWhiteSpace(label)) return "PL " + mm;
            string clean = label!.Trim();
            if (clean.StartsWith("PL", System.StringComparison.OrdinalIgnoreCase)) clean = clean.Substring(2).Trim();
            return "PL " + clean + " · " + mm;
        }

        /// <summary>Texto de una soldadura de filete: "soldadura 5,0 mm todo el contorno".</summary>
        public static string Weld(double? sizeMm, bool allAround)
        {
            string size = sizeMm.HasValue ? Mm(sizeMm.Value) + " mm" : "tamaño sin definir";
            return "soldadura " + size + (allAround ? " todo el contorno" : "");
        }
    }
}
