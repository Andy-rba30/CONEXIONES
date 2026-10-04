using System;
using System.Collections.Generic;
using System.Globalization;

namespace MotorConexiones.Core.Editing
{
    /// <summary>Lee y escribe los textos de la tabla: números con coma o punto, listas con punto y coma, sí/no.</summary>
    public static class SpecValueParser
    {
        public static bool TryParseNumber(string? text, out double value)
        {
            value = 0.0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            string t = text!.Trim().Replace(" ", "");
            // "9,525" → "9.525"; "1.234,5" (miles con punto) → "1234.5".
            if (t.Contains(",") && t.Contains("."))
            {
                t = t.Replace(".", "").Replace(',', '.');
            }
            else
            {
                t = t.Replace(',', '.');
            }
            return double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && !double.IsNaN(value) && !double.IsInfinity(value);
        }

        public static bool TryParseInteger(string? text, out int value)
        {
            value = 0;
            if (!TryParseNumber(text, out double d)) return false;
            if (Math.Abs(d - Math.Round(d)) > 1e-9 || d > int.MaxValue || d < int.MinValue) return false;
            value = (int)Math.Round(d);
            return true;
        }

        public static bool TryParseLong(string? text, out long value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            return long.TryParse(text!.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        public static bool TryParseBoolean(string? text, out bool value)
        {
            value = false;
            if (string.IsNullOrWhiteSpace(text)) return false;
            switch (text!.Trim().ToLowerInvariant())
            {
                case "sí":
                case "si":
                case "s":
                case "true":
                case "verdadero":
                case "1":
                case "x":
                    value = true;
                    return true;
                case "no":
                case "n":
                case "false":
                case "falso":
                case "0":
                    value = false;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary><c>75; 420; 70</c> (también admite saltos de línea o barras) → lista de números.</summary>
        public static bool TryParseNumberList(string? text, out List<double> values)
        {
            values = new List<double>();
            if (string.IsNullOrWhiteSpace(text)) return true;
            foreach (string part in text!.Split(new[] { ';', '\n', '\r', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrWhiteSpace(part)) continue;
                if (!TryParseNumber(part, out double v)) return false;
                values.Add(v);
            }
            return true;
        }

        /// <summary><c>-175; 280</c> → (x, y). Si no hay punto y coma, admite dos números separados por espacio.</summary>
        public static bool TryParsePoint(string? text, out double x, out double y)
        {
            x = 0.0;
            y = 0.0;
            if (string.IsNullOrWhiteSpace(text)) return false;
            string[] parts = text!.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                parts = text.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2) return false;
            }
            return TryParseNumber(parts[0], out x) && TryParseNumber(parts[1], out y);
        }

        public static string FormatNumber(double? value)
        {
            if (!value.HasValue) return "";
            return value.Value.ToString("0.###", CultureInfo.InvariantCulture).Replace('.', ',');
        }

        public static string FormatInteger(int? value)
        {
            return value.HasValue ? value.Value.ToString(CultureInfo.InvariantCulture) : "";
        }

        public static string FormatBoolean(bool value) => value ? "sí" : "no";

        public static string FormatNumberList(IEnumerable<double>? values)
        {
            if (values == null) return "";
            var parts = new List<string>();
            foreach (double v in values) parts.Add(FormatNumber(v));
            return string.Join("; ", parts);
        }

        public static string FormatPoint(double[]? point)
        {
            if (point == null || point.Length < 2) return "";
            return FormatNumber(point[0]) + "; " + FormatNumber(point[1]);
        }
    }
}
