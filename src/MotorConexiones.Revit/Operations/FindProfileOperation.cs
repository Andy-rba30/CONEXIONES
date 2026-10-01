using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_find_profile</c>: busca tipos de perfil cargados en el modelo que coincidan con una designación AISC
    /// (por ejemplo <c>HSS2-1/2X2-1/2X3/16</c>).
    /// </summary>
    public sealed class FindProfileOperation : IOperation
    {
        public string Name => "find_profile";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            string query = "";

            if (context.TryGet("query", out var qEl) && qEl.ValueKind == JsonValueKind.String)
            {
                query = qEl.GetString() ?? "";
            }
            else if (context.TryGet("name", out var nEl) && nEl.ValueKind == JsonValueKind.String)
            {
                query = nEl.GetString() ?? "";
            }
            else if (context.TryGet("profile", out var pEl) && pEl.ValueKind == JsonValueKind.String)
            {
                query = pEl.GetString() ?? "";
            }

            var collector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralFraming)
                .WhereElementIsElementType();

            var matches = new List<object>();
            var allNames = new List<string>();

            foreach (ElementType elem in collector)
            {
                string typeName = elem.Name;
                string familyName = elem.get_Parameter(BuiltInParameter.ALL_MODEL_FAMILY_NAME)?.AsString() ?? "";
                allNames.Add(typeName);

                if (!string.IsNullOrWhiteSpace(query))
                {
                    if (ProfileMatcher.Matches(query, typeName, 0, 0, 0) ||
                        typeName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        familyName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        matches.Add(new
                        {
                            type_name = typeName,
                            family_name = familyName,
                            exact_match = string.Equals(typeName, query, StringComparison.OrdinalIgnoreCase)
                        });
                    }
                }
            }

            // Si no hubo coincidencia directa pero hay consulta, buscar sugerencias por similitud
            IReadOnlyList<string> suggestions = Array.Empty<string>();
            if (matches.Count == 0 && !string.IsNullOrWhiteSpace(query))
            {
                suggestions = ProfileMatcher.GetSuggestions(query, allNames);
            }

            var data = new
            {
                query = query,
                total_profiles_in_model = allNames.Count,
                matched_count = matches.Count,
                matches = matches,
                suggestions = suggestions
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
