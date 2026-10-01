using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Units;

namespace MotorConexiones.Core.Validation
{
    /// <summary>
    /// Genera y verifica el validation_token canónico y determinista mediante SHA-256 (sección 5.4 del encargo).
    /// </summary>
    public static class ValidationTokenGenerator
    {
        /// <summary>
        /// Genera el token SHA-256 combinando:
        /// 1. Especificación en JSON canónico (claves ordenadas alfabéticamente, sin espacios).
        /// 2. Hash de configuración de límites y tolerancias (si se proporciona).
        /// 3. ProjectInformation.UniqueId del documento.
        /// 4. Por cada element_id implicado (ordenados): UniqueId, TypeName y extremos de curva redondeados a 0.1 mm.
        /// </summary>
        public static string GenerateToken(string specJson, IModelFacts? modelFacts, IEnumerable<long>? elementIds = null, LimitsConfig? limits = null)
        {
            if (string.IsNullOrWhiteSpace(specJson))
                throw new ArgumentException("El JSON de la especificación no puede estar vacío.", nameof(specJson));

            string canonicalSpec = ToCanonicalJson(specJson);

            var sb = new StringBuilder();
            sb.Append(canonicalSpec);

            if (limits != null)
            {
                sb.Append("|LIMITS:").Append(limits.ComputeHash());
            }

            if (modelFacts != null)
            {
                sb.Append("|PROJECT:").Append(modelFacts.ProjectUniqueId);

                // Determinar qué IDs incluir
                var ids = new HashSet<long>();
                if (elementIds != null)
                {
                    foreach (var id in elementIds) ids.Add(id);
                }
                else
                {
                    var spec = ConnectionSpec.FromJson(specJson);
                    if (spec?.Node?.ElementIds != null)
                    {
                        foreach (var id in spec.Node.ElementIds) ids.Add(id);
                    }
                    if (spec?.Chord != null && spec.Chord.ElementId > 0)
                    {
                        ids.Add(spec.Chord.ElementId);
                    }
                }

                foreach (var id in ids.OrderBy(x => x))
                {
                    var facts = modelFacts.GetMemberFacts(id);
                    if (facts != null)
                    {
                        double sx = UnitConverter.RoundMm(facts.CurveStartMm.X);
                        double sy = UnitConverter.RoundMm(facts.CurveStartMm.Y);
                        double sz = UnitConverter.RoundMm(facts.CurveStartMm.Z);
                        double ex = UnitConverter.RoundMm(facts.CurveEndMm.X);
                        double ey = UnitConverter.RoundMm(facts.CurveEndMm.Y);
                        double ez = UnitConverter.RoundMm(facts.CurveEndMm.Z);

                        sb.Append("|MEMBER:").Append(id)
                          .Append(':').Append(facts.UniqueId)
                          .Append(':').Append(facts.TypeName)
                          .Append(':').Append(sx.ToString("0.0", CultureInfo.InvariantCulture))
                          .Append(',').Append(sy.ToString("0.0", CultureInfo.InvariantCulture))
                          .Append(',').Append(sz.ToString("0.0", CultureInfo.InvariantCulture))
                          .Append(':').Append(ex.ToString("0.0", CultureInfo.InvariantCulture))
                          .Append(',').Append(ey.ToString("0.0", CultureInfo.InvariantCulture))
                          .Append(',').Append(ez.ToString("0.0", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        sb.Append("|MEMBER:").Append(id).Append(":MISSING");
                    }
                }
            }

            return ComputeSha256Hex(sb.ToString());
        }

        public static string GenerateToken(ConnectionSpec spec, IModelFacts? modelFacts, LimitsConfig? limits = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            string json = spec.ToJson();
            return GenerateToken(json, modelFacts, spec.Node?.ElementIds, limits);
        }

        /// <summary>
        /// Serializa un JSON arbitrario en su representación canónica (claves ordenadas alfabéticamente en todos los niveles, sin espacios).
        /// </summary>
        public static string ToCanonicalJson(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var sb = new StringBuilder();
            WriteCanonicalElement(doc.RootElement, sb);
            return sb.ToString();
        }

        private static void WriteCanonicalElement(JsonElement element, StringBuilder sb)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    sb.Append('{');
                    var properties = element.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal).ToList();
                    for (int i = 0; i < properties.Count; i++)
                    {
                        if (i > 0) sb.Append(',');
                        sb.Append('"').Append(EscapeString(properties[i].Name)).Append("\":");
                        WriteCanonicalElement(properties[i].Value, sb);
                    }
                    sb.Append('}');
                    break;

                case JsonValueKind.Array:
                    sb.Append('[');
                    int count = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        if (count > 0) sb.Append(',');
                        WriteCanonicalElement(item, sb);
                        count++;
                    }
                    sb.Append(']');
                    break;

                case JsonValueKind.String:
                    sb.Append('"').Append(EscapeString(element.GetString() ?? string.Empty)).Append('"');
                    break;

                case JsonValueKind.Number:
                    if (element.TryGetInt64(out long l))
                    {
                        sb.Append(l.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        sb.Append(element.GetDouble().ToString("G17", CultureInfo.InvariantCulture));
                    }
                    break;

                case JsonValueKind.True:
                    sb.Append("true");
                    break;

                case JsonValueKind.False:
                    sb.Append("false");
                    break;

                case JsonValueKind.Null:
                    sb.Append("null");
                    break;
            }
        }

        private static string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }

        private static string ComputeSha256Hex(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(bytes);
            var hex = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                hex.Append(b.ToString("x2"));
            }
            return hex.ToString();
        }
    }
}
