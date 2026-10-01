using System.Text.Json;
using MotorConexiones.Core;
using MotorConexiones.Core.Contract;
using Xunit;

namespace MotorConexiones.Tests
{
    public class ApiResponseTests
    {
        [Fact]
        public void Failure_SerializesEnvelopeOfSection10()
        {
            var response = ApiResponse.Failure("validate",
                new ApiError("BOLT_EDGE_DISTANCE_TOO_SMALL", "La distancia al borde (15 mm) es menor que el mínimo.",
                    "members[2].attachment.bolts.edge_mm", "Usa edge_mm >= 22."));
            response.Meta.DurationMs = 41;

            using var json = JsonDocument.Parse(response.ToJson());
            var root = json.RootElement;

            Assert.False(root.GetProperty("ok").GetBoolean());
            Assert.Equal(JsonValueKind.Null, root.GetProperty("data").ValueKind);
            var error = root.GetProperty("errors")[0];
            Assert.Equal("BOLT_EDGE_DISTANCE_TOO_SMALL", error.GetProperty("code").GetString());
            Assert.Equal("members[2].attachment.bolts.edge_mm", error.GetProperty("path").GetString());
            Assert.Contains("mínimo", error.GetProperty("message").GetString());
            Assert.Equal("Usa edge_mm >= 22.", error.GetProperty("hint").GetString());
            Assert.Equal(0, root.GetProperty("warnings").GetArrayLength());
            Assert.Equal("validate", root.GetProperty("meta").GetProperty("operation").GetString());
            Assert.Equal(41, root.GetProperty("meta").GetProperty("duration_ms").GetInt64());
            Assert.Equal(AddinInfo.Version, root.GetProperty("meta").GetProperty("addin_version").GetString());
        }

        [Fact]
        public void Success_KeepsAccentsUnescaped()
        {
            var response = ApiResponse.Success("ping", new { mensaje = "Añadir conexión" });
            string text = response.ToJson();
            Assert.Contains("Añadir conexión", text);
            Assert.DoesNotContain("\\u00", text);
        }

        [Fact]
        public void Success_DataUsesSnakeCaseForAnonymousObjects()
        {
            var response = ApiResponse.Success("ping", new { AddinVersion = "0.1.0", RevitBuild = "27.2" });
            using var json = JsonDocument.Parse(response.ToJson());
            Assert.Equal("0.1.0", json.RootElement.GetProperty("data").GetProperty("addin_version").GetString());
        }
    }
}
