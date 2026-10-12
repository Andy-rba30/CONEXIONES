using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Schema;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Types
{
    /// <summary>
    /// Fase 12: el tipo <c>direct_weld</c>, el nudo en K soldado del cordón superior del Hangar (decisión P3): cada diagonal
    /// se corta contra la cara del cordón y se suelda en todo el contorno; sin cartela, sin placas, sin pernos. Admite la
    /// excentricidad del nudo (opción A) y, en un empalme, que cada diagonal llegue a un tramo distinto (D5).
    /// </summary>
    public sealed class DirectWeldType : IConnectionType, ISketchProvider
    {
        public static DirectWeldType Instance { get; } = new DirectWeldType();

        public string Name => DirectWeldValidator.TypeName;

        public string Description => "Nudo en K soldado: cada diagonal o montante HSS se corta contra la cara del cordón y se suelda en todo el contorno, sin cartela ni placas (los nudos del cordón superior de la cercha). Admite la excentricidad entre diagonales hasta medio canto del cordón.";

        public string GetSchemaJson() => JsonSchemaValidator.GetDirectWeldSchemaJson();

        public Sketch.Sketch BuildSketch(ConnectionSpec spec, SketchNodeInfo nodeInfo, LimitsConfig? limits = null) => DirectWeldSketch.Build(spec, nodeInfo, limits);

        public ValidationResult Validate(string? rawJson, ConnectionSpec? spec, IModelFacts? modelFacts = null, LimitsConfig? limits = null) =>
            SpecValidator.Validate(rawJson, spec, modelFacts, limits);

        public string GetExampleJson()
        {
            return @"{
  ""spec_version"": ""1.0"",
  ""connection_type"": ""direct_weld"",
  ""source"": { ""drawing"": ""Nudo del cordón superior (alzado del módulo típico 03)"", ""scale"": ""1/10"" },
  ""node"": { ""element_ids"": [1245533, 1249631, 1249632], ""eccentricity_mm"": 34.0 },
  ""chord"": { ""element_id"": 1245533, ""profile"": ""HSS12X8X1/2"", ""continuous"": true, ""face"": ""auto"" },
  ""members"": [
    {
      ""element_id"": 1249631,
      ""role"": ""diagonal"",
      ""profile"": ""HSS2-1/2X2-1/2X3/16"",
      ""expected_angle_deg"": 44.4,
      ""axis_offset_mm"": -17.0,
      ""end_cut"": ""to_chord_face"",
      ""end_gap_mm"": 0.0,
      ""weld"": { ""type"": ""fillet"", ""size_mm"": 5.0, ""all_around"": true },
      ""splice_hint"": { ""detail"": ""B"", ""offset_from_chord_face_mm"": 330.0 }
    },
    {
      ""element_id"": 1249632,
      ""role"": ""diagonal"",
      ""profile"": ""HSS2-1/2X2-1/2X3/16"",
      ""expected_angle_deg"": 45.0,
      ""axis_offset_mm"": 17.0,
      ""end_cut"": ""to_chord_face"",
      ""end_gap_mm"": 0.0,
      ""weld"": { ""type"": ""fillet"", ""size_mm"": 5.0, ""all_around"": true },
      ""splice_hint"": { ""detail"": ""B"", ""offset_from_chord_face_mm"": 330.0 }
    }
  ],
  ""dimension_chains"": [],
  ""uncertain_fields"": []
}";
        }
    }
}
