using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Schema;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Types
{
    /// <summary>
    /// Fase 12: el tipo <c>end_plate</c>, la placa de extremo: dos planchas iguales cara a cara, perpendiculares al eje de la
    /// barra, cada una soldada a su tramo y unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I
    /// del plano). En esta fase, <c>kind: splice</c> (empalme de dos tramos de la misma barra).
    /// </summary>
    public sealed class EndPlateType : IConnectionType, ISketchProvider
    {
        public static EndPlateType Instance { get; } = new EndPlateType();

        public string Name => EndPlateValidator.TypeName;

        public string Description => "Placa de extremo (empalme): dos planchas con las esquinas recortadas, perpendiculares al eje y soldadas a cada tramo del tubo, unidas por pernos alrededor del tubo con tuerca a ambos lados (Detalles B, C e I: empalmes de la diagonal y de los cordones).";

        public string GetSchemaJson() => JsonSchemaValidator.GetEndPlateSchemaJson();

        public Sketch.Sketch BuildSketch(ConnectionSpec spec, SketchNodeInfo nodeInfo, LimitsConfig? limits = null) => EndPlateSketch.Build(spec, nodeInfo, limits);

        public ValidationResult Validate(string? rawJson, ConnectionSpec? spec, IModelFacts? modelFacts = null, LimitsConfig? limits = null) =>
            SpecValidator.Validate(rawJson, spec, modelFacts, limits);

        public string GetExampleJson()
        {
            return @"{
  ""spec_version"": ""1.0"",
  ""connection_type"": ""end_plate"",
  ""kind"": ""splice"",
  ""source"": { ""drawing"": ""Detalle C (empalme del cordón superior)"", ""scale"": ""1/10"" },
  ""node"": { ""element_ids"": [1245531, 1245530] },
  ""chord"": { ""element_id"": 1245531, ""profile"": ""HSS12X8X1/2"", ""continuous"": false, ""end_setback_mm"": 12.7 },
  ""members"": [
    { ""element_id"": 1245530, ""role"": ""chord"", ""profile"": ""HSS12X8X1/2"", ""end_setback_mm"": 12.7, ""expected_angle_deg"": 0.0 }
  ],
  ""plate"": {
    ""thickness_mm"": 12.7, ""thickness_label"": ""1/2\"""", ""width_mm"": 430.0, ""height_mm"": 330.0, ""corner_cut_mm"": 60.0,
    ""weld"": { ""type"": ""fillet"", ""size_mm"": 6.0, ""all_around"": true }
  },
  ""bolts"": {
    ""diameter_mm"": 19.05, ""diameter_label"": ""3/4\"""", ""hole_mm"": 20.638, ""hole_label"": ""13/16\"""", ""nuts"": ""both_sides"",
    ""faces"": {
      ""top"": { ""count"": 4, ""spacing_mm"": 90.0, ""edge_mm"": 25.0 },
      ""bottom"": { ""count"": 4, ""spacing_mm"": 90.0, ""edge_mm"": 25.0 },
      ""left"": { ""count"": 2, ""spacing_mm"": 80.0, ""edge_mm"": 25.0 },
      ""right"": { ""count"": 2, ""spacing_mm"": 80.0, ""edge_mm"": 25.0 }
    }
  },
  ""gap_mm"": 0.0,
  ""dimension_chains"": [
    { ""label"": ""ancho de la plancha"", ""values_mm"": [60.0, 310.0, 60.0], ""expected_total_mm"": 430.0 },
    { ""label"": ""pernos de arriba"", ""values_mm"": [20.0, 90.0, 90.0, 90.0, 20.0], ""expected_total_mm"": 310.0 }
  ],
  ""uncertain_fields"": []
}";
        }
    }
}
