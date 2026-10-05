using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Ronda 8c: los textos de la ventana y de la respuesta del MCP salen del Core (<see cref="PlanAdvice"/>): estado en
    /// español con icono y color, visibilidad por defecto, consejo por caso y la cabecera con la decisión.
    /// </summary>
    public class PlanAdviceTests
    {
        private static PlanNode Node(string status, params ApiError[] warnings)
        {
            var node = new PlanNode { Name = "N4", Status = status, ChordElementId = 1250933, ElementIds = new List<long> { 1250933, 1251053, 1251054, 1251059 } };
            node.Warnings.AddRange(warnings);
            return node;
        }

        private static ApiError ChordMissing() => new ApiError(ErrorCodes.NodeChordNotContinuous, "Ninguna barra atraviesa el nudo.", "nodes[N4].chord_element_id");

        [Fact]
        public void Ready_IsGreenVisibleAndNothingToDo()
        {
            PlanNode node = Node(NodeStatus.Ready);
            node.Orientation = "same";
            node.TemplateName = "Nudo tipico Detalle D";
            Assert.Equal("● Listo", PlanAdvice.StatusText(node));
            Assert.Equal(PlanAdvice.Green, PlanAdvice.ColorName(node));
            Assert.Equal(new[] { 46, 160, 67 }, PlanAdvice.ColorRgb(node));
            Assert.True(PlanAdvice.VisibleByDefault(node));
            Assert.Equal("—", PlanAdvice.Advice(node));
            Assert.Equal("no", PlanAdvice.MirrorText(node));
            Assert.Equal("N4 · Listo · Nudo tipico Detalle D · igual", PlanAdvice.MapLabel(node));
        }

        [Fact]
        public void ReadyWithWarnings_IsAmberAndSaysWhatWillHappen()
        {
            PlanNode profile = Node(NodeStatus.Ready, new ApiError(ErrorCodes.TemplateProfileDiffers,
                "El perfil de el cordón en el modelo es 'HSS4X4X3-16 102x102' y la plantilla esperaba 'HSS3X3X1/4': se escribe el del modelo.", "chord.profile"));
            profile.Orientation = "mirror_x";
            profile.IsMirrored = true;
            Assert.Equal("▲ Listo con aviso", PlanAdvice.StatusText(profile));
            Assert.Equal(PlanAdvice.Amber, PlanAdvice.ColorName(profile));
            Assert.Equal("El cordón es HSS4X4X3-16 102x102 y la plantilla HSS3X3X1/4: se creará con la misma cartela; exclúyelo si no quieres", PlanAdvice.Advice(profile));
            Assert.Equal("sí", PlanAdvice.MirrorText(profile));
            Assert.EndsWith("· en espejo", PlanAdvice.MapLabel(profile));

            PlanNode angle = Node(NodeStatus.Ready, new ApiError(ErrorCodes.TemplateAngleDeviation, "La barra 1 llega a 51,2° y la plantilla la tenía a 45,0°.", "members[0].expected_angle_deg"));
            angle.MaxDeviationDeg = 6.2;
            Assert.Equal(PlanAdvice.Amber, PlanAdvice.ColorName(angle));
            Assert.StartsWith("Una barra se desvía 6.2° de la plantilla: se creará con el ángulo real", PlanAdvice.Advice(angle));

            PlanNode edited = Node(NodeStatus.Ready);
            edited.HasSpecOverride = true;
            edited.ReplacesExisting = true;
            edited.ExistingConnectionId = "conn-1";
            Assert.Equal("● Listo (editado) (rehacer)", PlanAdvice.StatusText(edited));
            Assert.Equal("Rehace la conexión existente conn-1", PlanAdvice.Advice(edited));
        }

        [Fact]
        public void Invalid_IsRedAndPointsToEditNode()
        {
            PlanNode node = Node(NodeStatus.Invalid);
            node.Errors.Add(new ApiError(ErrorCodes.PlateOutsideGusset, "La placa cuchilla de la barra 2 se sale de la cartela.", "members[1]"));
            Assert.Equal("✖ No valida", PlanAdvice.StatusText(node));
            Assert.Equal(PlanAdvice.Red, PlanAdvice.ColorName(node));
            Assert.True(PlanAdvice.VisibleByDefault(node));
            Assert.Equal("Una barra se sale de la cartela: Editar nudo y agrandarla, o excluir", PlanAdvice.Advice(node));

            PlanNode other = Node(NodeStatus.Invalid);
            other.Errors.Add(new ApiError(ErrorCodes.DimensionChainMismatch, "La cadena no suma.", "dimension_chains[0]"));
            Assert.Equal("No valida (DIMENSION_CHAIN_MISMATCH): Editar nudo para corregirlo, o excluir", PlanAdvice.Advice(other));
        }

        [Fact]
        public void NoMatch_WithChord_StaysVisibleAndAsksForATemplate()
        {
            PlanNode node = Node(NodeStatus.NoMatch);
            node.StatusDetail = "Ninguna plantilla casa todas sus ranuras con las barras del nudo.";
            node.Members.Add(new PlanMember { ElementId = 1251053, AngleDeg = 136.9 });
            node.Members.Add(new PlanMember { ElementId = 1251054, AngleDeg = 44.4 });
            node.Members.Add(new PlanMember { ElementId = 1251059, AngleDeg = -135.6 });
            Assert.Equal("✖ Sin plantilla que encaje", PlanAdvice.StatusText(node));
            Assert.Equal(PlanAdvice.Red, PlanAdvice.ColorName(node));
            Assert.True(PlanAdvice.VisibleByDefault(node));
            Assert.Equal("Ninguna plantilla encaja (3 barras, ángulos 136.9°, 44.4°, -135.6°): crea esa típica o excluye", PlanAdvice.Advice(node));
            Assert.Equal("", PlanAdvice.MirrorText(node));

            PlanNode byPerson = Node(NodeStatus.NoMatch);
            byPerson.StatusDetail = "Sin plantilla por decisión de la persona (template: null).";
            Assert.StartsWith("Sin plantilla por decisión tuya", PlanAdvice.Advice(byPerson));
        }

        [Fact]
        public void NoMatch_WithoutChord_IsHiddenWhenItIsAPairAndVisibleWhenItIsATrio()
        {
            PlanNode pair = Node(NodeStatus.NoMatch, ChordMissing());
            pair.ElementIds = new List<long> { 1251049, 1251050 };
            Assert.Equal("✖ Falta el cordón", PlanAdvice.StatusText(pair));
            Assert.Equal(PlanAdvice.Red, PlanAdvice.ColorName(pair));
            Assert.False(PlanAdvice.VisibleByDefault(pair));
            Assert.True(PlanAdvice.IsPairWithoutChord(pair));
            Assert.Equal("Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…", PlanAdvice.Advice(pair));

            PlanNode trio = Node(NodeStatus.NoMatch, ChordMissing());
            trio.ElementIds = new List<long> { 1251049, 1251050, 1251051 };
            Assert.True(PlanAdvice.VisibleByDefault(trio));
            Assert.False(PlanAdvice.IsPairWithoutChord(trio));
            Assert.Equal("✖ Falta el cordón", PlanAdvice.StatusText(trio));
        }

        [Fact]
        public void OtherStatuses_HaveTheirTextColorAndAdvice()
        {
            PlanNode ambiguous = Node(NodeStatus.AmbiguousChord);
            Assert.Equal("✖ Dos cordones posibles", PlanAdvice.StatusText(ambiguous));
            Assert.Equal(PlanAdvice.Red, PlanAdvice.ColorName(ambiguous));
            Assert.Equal("Elige el cordón con Cordón…", PlanAdvice.Advice(ambiguous));

            PlanNode offset = Node(NodeStatus.Offset);
            offset.StatusDetail = "El eje del cordón y el del primer miembro no se cortan: distan 12.3 mm (máximo 5.0 mm).";
            Assert.Equal("✖ Los ejes no se cortan", PlanAdvice.StatusText(offset));
            Assert.Equal("Los ejes se cruzan a 12.3 mm: corrige el modelo o excluye", PlanAdvice.Advice(offset));

            PlanNode already = Node(NodeStatus.AlreadyConnected);
            Assert.Equal("◌ Ya tiene conexión", PlanAdvice.StatusText(already));
            Assert.Equal(PlanAdvice.Gray, PlanAdvice.ColorName(already));
            Assert.True(PlanAdvice.VisibleByDefault(already));
            Assert.Contains("rehacer existentes", PlanAdvice.Advice(already));

            PlanNode excluded = Node(NodeStatus.Excluded);
            Assert.Equal("◌ Excluido", PlanAdvice.StatusText(excluded));
            Assert.Equal(PlanAdvice.Gray, PlanAdvice.ColorName(excluded));
            Assert.True(PlanAdvice.VisibleByDefault(excluded));
            Assert.Contains("Incluir", PlanAdvice.Advice(excluded));

            PlanNode loose = Node(NodeStatus.Untyped);
            loose.ElementIds = new List<long> { 1251049 };
            Assert.Equal("○ Barra suelta (no es nudo)", PlanAdvice.StatusText(loose));
            Assert.Equal(PlanAdvice.Gray, PlanAdvice.ColorName(loose));
            Assert.False(PlanAdvice.VisibleByDefault(loose));
            Assert.True(PlanAdvice.IsLooseBar(loose));
            Assert.Equal("No es un nudo: nada que hacer", PlanAdvice.Advice(loose));

            Assert.Equal(new[] { 240, 160, 0 }, new[] { (int)PlanAdvice.Rgb(PlanAdvice.Amber).R, (int)PlanAdvice.Rgb(PlanAdvice.Amber).G, (int)PlanAdvice.Rgb(PlanAdvice.Amber).B });
            Assert.Equal("ámbar", PlanAdvice.ColorLabel(PlanAdvice.Amber));
            Assert.Equal(3, PlanAdvice.ColorIndexOf(PlanAdvice.Gray));
            Assert.Contains("Verde = se creará", PlanAdvice.Legend);
        }

        [Fact]
        public void SummaryText_HangarTruss8b_SaysWhatWillBeCreatedAndWhatIsHidden()
        {
            // El plan del paso 8b-4 (59 nudos: 16 ready, 20 parejas sin cordón, 23 barras sueltas), en una cabecera que se entiende.
            BatchPlan plan = PlanBuilder.Build(BatchPlanTests.Hangar8bRequest());
            Assert.Equal("Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas.",
                PlanAdvice.SummaryText(plan));
            Assert.Equal(16, PlanAdvice.VisibleCount(plan));
            Assert.Equal(20, PlanAdvice.HiddenPairsWithoutChord(plan));
            Assert.Equal(23, PlanAdvice.HiddenLooseBars(plan));
            Assert.Equal("20 sin cordón, 23 barras sueltas", PlanAdvice.HiddenText(plan));
            Assert.False(PlanAdvice.IsCatalogEmpty(plan));

            // Colores por estado (decisión P4): 14 ámbar (perfil distinto), 2 verdes (el tramo HSS3X3), 20 rojos ocultos, 23 grises ocultos.
            PlanAdvice.ApplyStatusColors(plan);
            Assert.Equal(14, plan.Nodes.Count(n => n.ColorName == PlanAdvice.Amber));
            Assert.Equal(2, plan.Nodes.Count(n => n.ColorName == PlanAdvice.Green));
            Assert.Equal(20, plan.Nodes.Count(n => n.ColorName == PlanAdvice.Red));
            Assert.Equal(23, plan.Nodes.Count(n => n.ColorName == PlanAdvice.Gray));
            Assert.All(plan.Nodes, n => Assert.Equal(PlanAdvice.ColorRgb(n), n.ColorRgb));
            Assert.All(plan.Nodes, n => Assert.Equal(PlanAdvice.ColorIndexOf(n.ColorName), n.ColorIndex));
            Assert.Equal(16, plan.Nodes.Count(n => n.CanBeMarked));

            PlanNode n4 = plan.Find("N4")!;
            Assert.Equal("▲ Listo con aviso", PlanAdvice.StatusText(n4));
            Assert.StartsWith("El cordón es HSS4X4", PlanAdvice.Advice(n4, plan));
            Assert.Equal("no", PlanAdvice.MirrorText(n4));
            Assert.Equal("sí", PlanAdvice.MirrorText(plan.Find("N7")!));
            PlanNode small = plan.Nodes.First(n => n.Status == NodeStatus.Ready && n.ChordElementId == HangarTruss8b.SmallChord);
            Assert.Equal("● Listo", PlanAdvice.StatusText(small));
            Assert.Equal("—", PlanAdvice.Advice(small, plan));
        }

        [Fact]
        public void SummaryText_SyntheticTruss_CountsEveryVisibleProblem()
        {
            BatchPlan plan = PlanBuilder.Build(BatchPlanTests.Request());
            string text = PlanAdvice.SummaryText(plan);
            int ready = plan.Nodes.Count(n => n.Status == NodeStatus.Ready);
            int noMatch = plan.Nodes.Count(n => PlanAdvice.VisibleByDefault(n) && n.Status == NodeStatus.NoMatch && !PlanAdvice.ChordMissing(n));
            int invalid = plan.Nodes.Count(n => n.Status == NodeStatus.Invalid);
            Assert.StartsWith("Se crearán " + ready + " conexiones con Nudo típico Detalle D (", text);
            Assert.Contains(noMatch + " sin plantilla que encaje.", text);
            if (invalid > 0) Assert.Contains(invalid + (invalid == 1 ? " nudo no valida." : " nudos no validan."), text);
            Assert.Contains("Ocultos: " + PlanAdvice.HiddenText(plan) + ".", text);
            Assert.DoesNotContain("ready", text);
            Assert.DoesNotContain("no_match", text);
            Assert.DoesNotContain("untyped", text);
        }

        [Fact]
        public void EmptyCatalog_IsSaidInSpanishWithTheCatalogEmptyWarning()
        {
            // Sin ninguna plantilla todos los nudos salen no_match: la cabecera, el consejo y el aviso lo explican (C9).
            PlanRequest request = BatchPlanTests.Request();
            request.Templates = new List<CatalogTemplate>();
            BatchPlan plan = PlanBuilder.Build(request);
            Assert.True(PlanAdvice.IsCatalogEmpty(plan));
            Assert.Equal(0, plan.ReadyCount);
            Assert.StartsWith("Ningún nudo listo: no hay plantillas en el catálogo (crea primero la conexión de un nudo y guárdala con Guardar en catálogo).", PlanAdvice.SummaryText(plan));
            PlanNode node = plan.Nodes.First(n => n.Status == NodeStatus.NoMatch && !PlanAdvice.ChordMissing(n));
            Assert.Equal("✖ Sin plantillas en el catálogo", PlanAdvice.StatusText(node));
            Assert.Equal(PlanAdvice.CatalogEmptyAdvice, PlanAdvice.Advice(node, plan));
            Assert.Equal(PlanAdvice.CatalogEmptyAdvice, PlanAdvice.Advice(node));
            ApiError warning = PlanAdvice.CatalogEmptyWarning();
            Assert.Equal("CATALOG_EMPTY", warning.Code);
            Assert.Equal(ErrorCodes.CatalogEmpty, warning.Code);
            Assert.Contains("Guardar en catálogo", warning.Message);
        }

        [Fact]
        public void SummaryText_WithNothingReady_NamesTheMostFrequentCause()
        {
            var plan = new BatchPlan { Templates = { ["t1"] = "Plantilla A" } };
            PlanNode a = Node(NodeStatus.NoMatch);
            a.Name = "N1";
            a.StatusDetail = "Ninguna plantilla casa todas sus ranuras con las barras del nudo.";
            PlanNode b = Node(NodeStatus.NoMatch);
            b.Name = "N2";
            b.StatusDetail = a.StatusDetail;
            PlanNode c = Node(NodeStatus.Invalid);
            c.Name = "N3";
            PlanNode d = Node(NodeStatus.Untyped);
            d.Name = "N4";
            d.ElementIds = new List<long> { 1 };
            plan.Nodes.AddRange(new[] { a, b, c, d });
            Assert.Equal("Ningún nudo listo: ninguna plantilla encaja en 2 nudos. 1 nudo no valida. Ocultos: 1 barra suelta.", PlanAdvice.SummaryText(plan));

            var empty = new BatchPlan { Templates = { ["t1"] = "Plantilla A" } };
            Assert.Equal("Ningún nudo listo: no se detectó ningún nudo en la selección.", PlanAdvice.SummaryText(empty));

            var onlyHidden = new BatchPlan { Templates = { ["t1"] = "Plantilla A" } };
            onlyHidden.Nodes.Add(d);
            Assert.StartsWith("Ningún nudo listo: la selección solo tiene barras sueltas o parejas sin cordón", PlanAdvice.SummaryText(onlyHidden));
        }
    }
}
