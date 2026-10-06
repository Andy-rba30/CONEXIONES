using System;
using System.Collections.Generic;
using System.IO;
using MotorConexiones.Core.Brief;
using MotorConexiones.Core.Contract;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>Ronda 9b (Fase 10): el encargo para una IA externa, en un solo Markdown, escrito por el Core.</summary>
    public class DesignBriefTests
    {
        private static string Guide() => File.ReadAllText(SketchBuilderTests.FindRepoFile(Path.Combine("docs", "guide.md")));

        [Fact]
        public void Write_PutsEverythingInOrder()
        {
            var input = new DesignBriefInput
            {
                DocumentTitle = "HANGAR_PRUEBA_sondeo",
                AddinVersion = "0.10.0",
                CreatedLocal = new DateTime(2026, 10, 6, 15, 30, 0),
                SelectedElementIds = new List<long> { 1249510, 1249630, 1249631, 1249636 },
                NodeInfoJson = "{\n  \"chord_element_id\": 1249510\n}",
                SchemaExampleJson = "{\n  \"spec_version\": \"1.0\"\n}",
                GuideMarkdown = Guide(),
                ExampleJson = DesignBriefWriter.EmbeddedExampleJson(),
                ExampleSource = DesignBriefWriter.EmbeddedExampleSource,
            };
            string brief = DesignBriefWriter.Write(input);

            Assert.StartsWith("# Encargo para una IA externa: conexión gusset_node en HANGAR_PRUEBA_sondeo", brief);
            Assert.Contains("Generado por MotorConexiones 0.10.0 el 2026-10-06 15:30. Barras seleccionadas en Revit: 1249510, 1249630, 1249631, 1249636.", brief);
            Assert.Contains("## Prompt", brief);
            Assert.Contains(DesignBriefWriter.Prompt, brief);
            Assert.Contains("\"chord_element_id\": 1249510", brief);
            Assert.Contains("## Esquema (ejemplo lleno de conn_get_schema para gusset_node)", brief);
            Assert.Contains("## Cómo leer un detalle (docs/guide.md, secciones 2, 3 y 4)", brief);
            Assert.Contains("## 2. Cómo leer un detalle de acero", brief);
            Assert.Contains("## 3. Sistema de coordenadas y posiciones", brief);
            Assert.Contains("## 4. Qué hacer con cada error de `conn_validate`", brief);
            Assert.DoesNotContain("## 5. Catálogo de plantillas", brief);
            Assert.DoesNotContain("## 1. Flujo obligatorio", brief);
            Assert.Contains("## Ejemplo confirmado (el Detalle D de la Fase 3", brief);
            Assert.Contains("\"drawing\": \"Detalle D\"", brief);
            int prompt = brief.IndexOf("## Prompt", StringComparison.Ordinal);
            int node = brief.IndexOf("## Datos del nudo", StringComparison.Ordinal);
            int schema = brief.IndexOf("## Esquema", StringComparison.Ordinal);
            int guide = brief.IndexOf("## Cómo leer un detalle (docs", StringComparison.Ordinal);
            int example = brief.IndexOf("## Ejemplo confirmado", StringComparison.Ordinal);
            Assert.True(prompt < node && node < schema && schema < guide && guide < example);
            Assert.EndsWith("2. El JSON completo en un solo bloque de código, con los `element_id` de la sección \"Datos del nudo\".\n", brief.Replace("\r\n", "\n"));
        }

        [Fact]
        public void Write_SaysWhenSomethingIsMissing()
        {
            string brief = DesignBriefWriter.Write(new DesignBriefInput { DocumentTitle = "", GuideMarkdown = "" });
            Assert.Contains("en (sin documento)", brief);
            Assert.Contains("(no se pudieron leer los datos del nudo)", brief);
            Assert.Contains("(la guía docs/guide.md no está disponible", brief);
            Assert.Contains("(sin ejemplo)", brief);
            Assert.Contains("## Ejemplo confirmado (un JSON que ya se creó bien en Revit)", brief);
        }

        [Fact]
        public void ExtractGuideSections_TakesSections2To4()
        {
            string guide = "# Guía\n\n## 1. Flujo\nuno\n\n## 2. Leer\ndos\n\n## 3. Ejes\ntres\n\n## 4. Errores\ncuatro\n\n## 5. Catálogo\ncinco\n";
            Assert.Equal("## 2. Leer\ndos\n\n## 3. Ejes\ntres\n\n## 4. Errores\ncuatro", DesignBriefWriter.ExtractGuideSections(guide));
            Assert.StartsWith("(la guía no tiene la sección 2; se copia entera)", DesignBriefWriter.ExtractGuideSections("# Otra guía\nsin secciones"));
            Assert.Equal("## 2. Leer\ndos", DesignBriefWriter.ExtractGuideSections("## 2. Leer\ndos\n"));
        }

        [Fact]
        public void FileNameFor_IsSafe()
        {
            var when = new DateTime(2026, 10, 6, 15, 30, 0);
            Assert.Equal("encargo-HANGAR_PRUEBA_sondeo-20261006-1530.md", DesignBriefWriter.FileNameFor("HANGAR_PRUEBA_sondeo.rvt", when));
            Assert.Equal("encargo-Nave_2_prueba-20261006-1530.md", DesignBriefWriter.FileNameFor("Nave 2: prueba", when));
            Assert.Equal("encargo-documento-20261006-1530.md", DesignBriefWriter.FileNameFor("   ", when));
        }

        [Fact]
        public void EmbeddedExample_IsTheConfirmedDetalleD()
        {
            string json = DesignBriefWriter.EmbeddedExampleJson();
            ConnectionSpec spec = ConnectionSpec.FromJson(json)!;
            Assert.Equal("gusset_node", spec.ConnectionType);
            Assert.Equal(3, spec.Members.Count);
            // "Confirmado": las dudas del Detalle D llevan su user_confirmed_value (una plantilla no puede arrastrar dudas abiertas).
            Assert.Equal(2, spec.UncertainFields.Count);
            Assert.All(spec.UncertainFields, u => Assert.NotNull(u.UserConfirmedValue));
            Assert.Equal(File.ReadAllText(SketchBuilderTests.FindRepoFile(Path.Combine("docs", "fixtures", "detalle-D-confirmado.json"))).Replace("\r\n", "\n"), json.Replace("\r\n", "\n"));
        }
    }
}
