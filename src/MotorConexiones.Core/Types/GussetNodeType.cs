using System;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Schema;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Types
{
    /// <summary>
    /// Implementación de IConnectionType para el tipo "gusset_node" (nudo de cercha con cartela y cordón).
    /// </summary>
    public sealed class GussetNodeType : IConnectionType
    {
        public static GussetNodeType Instance { get; } = new GussetNodeType();

        public string Name => "gusset_node";

        public string Description => "Nudo de cercha con cartela plana, cordón continuo y diagonales/montantes HSS unidos por ranura soldada o placa cuchilla empernada.";

        public string GetSchemaJson() => JsonSchemaValidator.GetGussetNodeSchemaJson();

        public string GetExampleJson()
        {
            return @"{
  ""spec_version"": ""1.0"",
  ""connection_type"": ""gusset_node"",
  ""source"": { ""drawing"": ""Detalle D"", ""scale"": ""1/10"" },
  ""node"": { ""element_ids"": [1249510, 1249630, 1249631, 1249636] },
  ""chord"": { ""element_id"": 1249510, ""profile"": ""HSS3X3X1/4"", ""continuous"": true },
  ""gusset"": {
    ""thickness_mm"": 9.525,
    ""thickness_label"": ""3/8\"""",
    ""width_mm"": 565.0,
    ""height_mm"": 530.0,
    ""outline"": {
      ""mode"": ""polygon"",
      ""points_mm"": [
        [-175.0, 280.0], [245.0, 280.0], [315.0, 210.0], [315.0, -40.0],
        [-35.0, -250.0], [-125.0, -250.0], [-250.0, -115.0], [-250.0, 210.0]
      ]
    },
    ""chord_interface"": ""through_slot"",
    ""weld_to_chord"": { ""type"": ""fillet"", ""size_mm"": 5.0, ""all_around"": true }
  },
  ""members"": [
    {
      ""element_id"": 1249630,
      ""role"": ""diagonal"",
      ""profile"": ""HSS2-1/2X2-1/2X3/16"",
      ""end_setback_mm"": 180.0,
      ""expected_angle_deg"": 45.0,
      ""attachment"": {
        ""type"": ""welded_slot"",
        ""slot_length_mm"": 150.0,
        ""weld"": { ""type"": ""fillet"", ""size_mm"": 5.0, ""all_around"": true }
      }
    }
  ],
  ""dimension_chains"": [
    { ""label"": ""borde superior"", ""values_mm"": [75.0, 420.0, 70.0], ""expected_total_mm"": 565.0 }
  ],
  ""uncertain_fields"": []
}";
        }

        public ValidationResult Validate(string? rawJson, ConnectionSpec? spec, IModelFacts? modelFacts = null, LimitsConfig? limits = null)
        {
            return SpecValidator.Validate(rawJson, spec, modelFacts, limits);
        }
    }
}
