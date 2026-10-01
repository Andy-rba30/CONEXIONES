using System;
using System.IO;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Schema;
using MotorConexiones.Core.Types;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    public class SpecValidationTests
    {
        private static string GetFixtureJsonPath()
        {
            // Busca docs/fixtures/detalle-D.json relativo al ejecutable o al directorio base
            string current = AppDomain.CurrentDomain.BaseDirectory;
            while (!string.IsNullOrEmpty(current))
            {
                string candidate = Path.Combine(current, "docs", "fixtures", "detalle-D.json");
                if (File.Exists(candidate)) return candidate;
                string candidateUp = Path.Combine(current, "..", "docs", "fixtures", "detalle-D.json");
                if (File.Exists(candidateUp)) return Path.GetFullPath(candidateUp);
                string candidateUp3 = Path.Combine(current, "..", "..", "..", "..", "docs", "fixtures", "detalle-D.json");
                if (File.Exists(candidateUp3)) return Path.GetFullPath(candidateUp3);
                current = Path.GetDirectoryName(current)!;
            }
            throw new FileNotFoundException("No se encontró docs/fixtures/detalle-D.json");
        }

        private static string GetLimitsJsonPath()
        {
            string current = AppDomain.CurrentDomain.BaseDirectory;
            while (!string.IsNullOrEmpty(current))
            {
                string candidate = Path.Combine(current, "config", "limits.json");
                if (File.Exists(candidate)) return candidate;
                string candidateUp = Path.Combine(current, "..", "config", "limits.json");
                if (File.Exists(candidateUp)) return Path.GetFullPath(candidateUp);
                string candidateUp3 = Path.Combine(current, "..", "..", "..", "..", "config", "limits.json");
                if (File.Exists(candidateUp3)) return Path.GetFullPath(candidateUp3);
                current = Path.GetDirectoryName(current)!;
            }
            throw new FileNotFoundException("No se encontró config/limits.json");
        }

        private static (string rawJson, ConnectionSpec spec) LoadDetalleDFixture(bool confirmUncertainties = true)
        {
            string path = GetFixtureJsonPath();
            string json = File.ReadAllText(path);
            var spec = ConnectionSpec.FromJson(json)!;

            if (confirmUncertainties)
            {
                foreach (var u in spec.UncertainFields)
                {
                    if (u.Path == "members[1].profile")
                    {
                        u.UserConfirmedValue = "HSS2-1-2X2-1-2X3-16 64x64";
                    }
                    else if (u.Path == "gusset.chord_interface")
                    {
                        u.UserConfirmedValue = "through_slot";
                    }
                    else
                    {
                        u.UserConfirmedValue = "confirmed";
                    }
                }
            }

            return (spec.ToJson(), spec);
        }

        [Fact]
        public void DetalleD_ValidSpec_WithConfirmedUncertainties_ValidatesCleanlyAndProducesToken()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            var modelFacts = new FakeModelFacts();
            var limits = LimitsConfig.LoadFromFile(GetLimitsJsonPath());

            var result = SpecValidator.Validate(json, spec, modelFacts, limits);

            Assert.True(result.IsValid, $"Errores inesperados: {string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Message}"))}");
            Assert.Empty(result.Errors);
            Assert.NotNull(result.ValidationToken);
            Assert.Equal(64, result.ValidationToken.Length);
        }

        [Fact]
        public void DimensionChain_Mismatch_420_To_402_YieldsDimensionChainMismatch()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            // Borde superior: cambiar 420 por 402 mm (suma dará 547 en vez de 565)
            spec.DimensionChains[0].ValuesMm[1] = 402.0;

            var modelFacts = new FakeModelFacts();
            var result = SpecValidator.Validate(spec.ToJson(), spec, modelFacts);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.DimensionChainMismatch);
            var err = result.Errors.First(e => e.Code == ErrorCodes.DimensionChainMismatch);
            Assert.Contains("547.0 mm pero se esperaba 565.0 mm", err.Message);
            Assert.Null(result.ValidationToken);
        }

        [Fact]
        public void BoltEdgeDistance_15mm_YieldsBoltEdgeDistanceTooSmall()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            // Poner bordes de 15 mm en los pernos de 5/8" (el mínimo es 22 mm según AISC Tabla J3.4)
            var bolts = spec.Members[2].Attachment!.Bolts!;
            bolts.EdgeMm = 15.0;

            var modelFacts = new FakeModelFacts();
            var limits = LimitsConfig.LoadFromFile(GetLimitsJsonPath());
            var result = SpecValidator.Validate(spec.ToJson(), spec, modelFacts, limits);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.BoltEdgeDistanceTooSmall);
            var err = result.Errors.First(e => e.Code == ErrorCodes.BoltEdgeDistanceTooSmall);
            Assert.Contains("15.0 mm", err.Message);
            Assert.Contains("22.0 mm", err.Message);
            Assert.Null(result.ValidationToken);
        }

        [Fact]
        public void LabelValueMismatch_ThicknessLabel3_8_With12mm_YieldsLabelValueMismatch()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            // Poner thickness_label "3/8\"" con thickness_mm = 12 (3/8" = 9.525 mm != 12 mm)
            spec.Gusset!.ThicknessLabel = "3/8\"";
            spec.Gusset.ThicknessMm = 12.0;

            var modelFacts = new FakeModelFacts();
            var limits = LimitsConfig.LoadFromFile(GetLimitsJsonPath());
            var result = SpecValidator.Validate(spec.ToJson(), spec, modelFacts, limits);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.LabelValueMismatch);
            var err = result.Errors.First(e => e.Code == ErrorCodes.LabelValueMismatch);
            Assert.Contains("gusset.thickness_label", err.Path);
            Assert.Null(result.ValidationToken);
        }

        [Fact]
        public void UnresolvedUncertainty_PreventsValidationToken()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: false);
            // Deja las dudas sin confirmar (user_confirmed_value = null)

            var modelFacts = new FakeModelFacts();
            var result = SpecValidator.Validate(json, spec, modelFacts);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.UnresolvedUncertainty);
            Assert.Null(result.ValidationToken);
        }

        [Fact]
        public void JsonSchema_AcceptsFixtureAndRejectsUnknownProperty()
        {
            string fixtureJson = File.ReadAllText(GetFixtureJsonPath());

            // 1. Debe aceptar el fixture sin errores de esquema
            var errorsValid = JsonSchemaValidator.Validate(fixtureJson);
            Assert.Empty(errorsValid);

            // 2. Debe rechazar un campo desconocido en la raíz (additionalProperties: false)
            string invalidRootJson = fixtureJson.Replace("\"spec_version\": \"1.0\",", "\"spec_version\": \"1.0\", \"unknown_field\": 123,");
            var errorsUnknownRoot = JsonSchemaValidator.Validate(invalidRootJson);
            Assert.Contains(errorsUnknownRoot, e => e.Code == ErrorCodes.SchemaInvalid && e.Path == "unknown_field");

            // 3. Debe rechazar un campo desconocido anidado dentro de gusset
            string invalidGussetJson = fixtureJson.Replace("\"width_mm\": 565,", "\"width_mm\": 565, \"invented_property\": \"extra\",");
            var errorsUnknownGusset = JsonSchemaValidator.Validate(invalidGussetJson);
            Assert.Contains(errorsUnknownGusset, e => e.Code == ErrorCodes.SchemaInvalid && e.Path != null && e.Path.Contains("invented_property"));
        }

        [Fact]
        public void UnitConversion_MmToFeetToMm_ReversibleUnder1Micron()
        {
            double[] testValuesMm = { 0.1, 1.0, 9.525, 15.875, 76.2, 140.0, 565.0, 17423.0 };
            foreach (double original in testValuesMm)
            {
                double feet = UnitConverter.MmToFeet(original);
                double backToMm = UnitConverter.FeetToMm(feet);
                double diff = Math.Abs(original - backToMm);
                Assert.True(diff < 0.001, $"Conversión irreversible para {original} mm: diferencia {diff} mm >= 0.001 mm");
            }

            // Comprobación de conversión de pulgadas a mm
            Assert.Equal(25.4, UnitConverter.InchesToMm(1.0), precision: 6);
            Assert.Equal(9.525, UnitConverter.InchesToMm(3.0 / 8.0), precision: 6);
            Assert.Equal(15.875, UnitConverter.InchesToMm(5.0 / 8.0), precision: 6);
        }

        [Fact]
        public void Polygon2D_SelfIntersecting_YieldsOutlineInvalid()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            // Polígono cruzado en forma de ocho (auto-intersección de aristas)
            spec.Gusset!.Outline!.PointsMm = new System.Collections.Generic.List<double[]>
            {
                new double[] { 0, 0 },
                new double[] { 200, 200 },
                new double[] { 0, 200 },
                new double[] { 200, 0 }
            };

            var result = SpecValidator.Validate(spec.ToJson(), spec);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.OutlineInvalid);
        }

        [Fact]
        public void BoltPattern_OutsidePlate_YieldsBoltOutsidePlate()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            // Modificar placa cuchilla para que sea muy estrecha (50 mm de ancho)
            // mientras los pernos requieren (2-1)*60 + 2*40 = 140 mm
            spec.Members[2].Attachment!.Plate!.WidthMm = 50.0;

            var result = SpecValidator.Validate(spec.ToJson(), spec);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.BoltOutsidePlate);
        }

        [Fact]
        public void KnifePlate_OutsideGusset_YieldsPlateOutsideGusset()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            // Aumentar retiro de extremo a 800 mm, empujando la placa fuera de la cartela de 565x530 mm
            spec.Members[2].EndSetbackMm = 800.0;

            var result = SpecValidator.Validate(spec.ToJson(), spec);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.PlateOutsideGusset);
        }

        [Fact]
        public void BoltSpacing_TooSmall_YieldsBoltSpacingTooSmall()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            // Pernos de 5/8" (15.875 mm): min_spacing = 2.667 * 15.875 = 42.3 mm.
            // Establecer spacing_mm = 30 mm
            spec.Members[2].Attachment!.Bolts!.SpacingMm = 30.0;

            var result = SpecValidator.Validate(spec.ToJson(), spec);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Code == ErrorCodes.BoltSpacingTooSmall);
        }

        [Fact]
        public void Weld_BelowMinimum_YieldsWarningWeldBelowMinimum()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            // Espesor de cartela es 9.525 mm (rango 6 < t <= 13 mm -> filete mínimo AISC J2.4 = 5 mm).
            // Poner size_mm = 3 mm genera advertencia
            spec.Gusset!.WeldToChord!.SizeMm = 3.0;

            var modelFacts = new FakeModelFacts();
            var limits = LimitsConfig.LoadFromFile(GetLimitsJsonPath());
            var result = SpecValidator.Validate(spec.ToJson(), spec, modelFacts, limits);

            Assert.True(result.IsValid); // Las advertencias no invalidan el resultado
            Assert.Contains(result.Warnings, w => w.Code == ErrorCodes.WeldBelowMinimum);
            Assert.NotNull(result.ValidationToken);
        }

        [Fact]
        public void ProfileMatcher_Equivalence_HSSNormalized()
        {
            // Debe reconocer equivalencia de designación con guiones, barras y etiquetas métricas
            Assert.True(ProfileMatcher.Matches("HSS2-1/2X2-1/2X3/16", "HSS2-1-2X2-1-2X3-16 64x64"));
            Assert.True(ProfileMatcher.Matches("HSS3X3X1/4", "HSS3X3X1/4"));
            Assert.True(ProfileMatcher.Matches("HSS 2-1/2 x 2-1/2 x 3/16", "HSS2-1-2X2-1-2X3-16"));

            // Mismatch da false
            Assert.False(ProfileMatcher.Matches("HSS4X4X1/2", "HSS2-1-2X2-1-2X3-16 64x64"));

            // Sugerencias
            var available = new[] { "HSS2-1-2X2-1-2X3-16 64x64", "HSS3X3X1/4", "HSS4X4X3/8", "W12X26" };
            var suggestions = ProfileMatcher.GetSuggestions("HSS2-1/2X2-1/2X1/4", available, 3);
            Assert.NotEmpty(suggestions);
            Assert.Equal("HSS2-1-2X2-1-2X3-16 64x64", suggestions[0]);
        }

        [Fact]
        public void ModelFacts_Validations_ElementNotFound_And_AngleDiffers()
        {
            var (json, spec) = LoadDetalleDFixture(confirmUncertainties: true);
            var modelFacts = new FakeModelFacts();

            // 1. Elemento que no existe en el modelo
            spec.Node.ElementIds.Add(9999999);
            var resultNotFound = SpecValidator.Validate(spec.ToJson(), spec, modelFacts);
            Assert.False(resultNotFound.IsValid);
            Assert.Contains(resultNotFound.Errors, e => e.Code == ErrorCodes.ElementNotFound);

            // 2. Ángulo que difiere por más de 1 grado
            spec.Node.ElementIds.Remove(9999999);
            spec.Members[0].ExpectedAngleDeg = 48.0; // en modelo real es 45.0
            var resultAngle = SpecValidator.Validate(spec.ToJson(), spec, modelFacts);
            Assert.True(resultAngle.IsValid); // Es advertencia
            Assert.Contains(resultAngle.Warnings, w => w.Code == ErrorCodes.AngleDiffersFromModel);
        }

        [Fact]
        public void CanonicalJson_ProducesDeterministicSha256Token()
        {
            string jsonA = "{\"b\": 2, \"a\": 1, \"nested\": {\"z\": 9, \"y\": 8}}";
            string jsonB = "{\"a\": 1, \"nested\": {\"y\": 8, \"z\": 9}, \"b\": 2}";

            string canonicalA = ValidationTokenGenerator.ToCanonicalJson(jsonA);
            string canonicalB = ValidationTokenGenerator.ToCanonicalJson(jsonB);

            Assert.Equal(canonicalA, canonicalB);
            Assert.Equal("{\"a\":1,\"b\":2,\"nested\":{\"y\":8,\"z\":9}}", canonicalA);

            var modelFacts = new FakeModelFacts();
            string tokenA = ValidationTokenGenerator.GenerateToken(jsonA, modelFacts, new long[] { 1249510 });
            string tokenB = ValidationTokenGenerator.GenerateToken(jsonB, modelFacts, new long[] { 1249510 });

            Assert.Equal(tokenA, tokenB);
            Assert.Equal(64, tokenA.Length);
        }

        [Fact]
        public void LimitsConfig_LoadsFromRepositoryFile()
        {
            string path = GetLimitsJsonPath();
            var limits = LimitsConfig.LoadFromFile(path);

            Assert.Equal(1, limits.SchemaVersion);
            Assert.Equal(1.0, limits.DimensionChainToleranceMm);
            Assert.Equal(0.05, limits.LabelValueToleranceMm);
            Assert.Equal(1.0, limits.AngleToleranceDeg);
            Assert.Equal(5.0, limits.NodeAxisMaxDistanceMm);
            Assert.Equal(2.667, limits.Bolts.MinSpacingFactor);

            // Tablas AISC
            Assert.Equal(22.0, limits.GetMinBoltEdgeDistance(15.875)); // 5/8" -> 22 mm
            Assert.Equal(25.0, limits.GetMinBoltEdgeDistance(19.05));  // 3/4" -> 25 mm
            Assert.Equal(3.0, limits.GetMinWeldFilletSize(5.0));       // t <= 6 -> 3 mm
            Assert.Equal(5.0, limits.GetMinWeldFilletSize(10.0));      // 6 < t <= 13 -> 5 mm
        }

        [Fact]
        public void GussetNodeType_RegistersAndProvidesSchemaAndExample()
        {
            var nodeType = GussetNodeType.Instance;
            Assert.Equal("gusset_node", nodeType.Name);
            Assert.False(string.IsNullOrWhiteSpace(nodeType.Description));

            string schema = nodeType.GetSchemaJson();
            Assert.Contains("GussetNodeConnectionSpec", schema);
            Assert.Contains("additionalProperties", schema);

            string example = nodeType.GetExampleJson();
            Assert.Contains("gusset_node", example);

            var spec = ConnectionSpec.FromJson(example);
            Assert.NotNull(spec);
            Assert.Equal("gusset_node", spec!.ConnectionType);
        }
    }
}
