using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Fase 9 (mejora C3): los botones de la columna "Qué hacer" salen del Core (<see cref="PlanAdvice.Actions"/>), y el
    /// consejo del empalme del cordón (12.1 de la Fase 8): cuando el cordón termina en el nudo y otro tramo sigue por el otro
    /// lado, el consejo ya no manda a "selecciónalo y replanifica".
    /// </summary>
    public class PlanActionsTests
    {
        private static PlanNode Node(string status, params ApiError[] warnings)
        {
            var node = new PlanNode { Name = "N9", Status = status, ChordElementId = 1245530, ElementIds = new List<long> { 1245530, 1245531, 1251056, 1251057 } };
            node.Warnings.AddRange(warnings);
            return node;
        }

        private static ApiError ChordMissing() => new ApiError(ErrorCodes.NodeChordNotContinuous, "Ninguna barra atraviesa el nudo.", "nodes[N9].chord_element_id");

        private static string[] Keys(PlanNode node, BatchPlan? plan = null) => PlanAdvice.Actions(node, plan).Select(a => a.Key).ToArray();

        [Fact]
        public void Actions_FollowTheStateOfTheNode()
        {
            Assert.Empty(Keys(Node(NodeStatus.Ready)));
            PlanNode profile = Node(NodeStatus.Ready, new ApiError(ErrorCodes.TemplateProfileDiffers, "El perfil de el cordón es 'HSS4X4' y la plantilla esperaba 'HSS3X3'.", "chord.profile"));
            Assert.Equal(new[] { PlanAction.Exclude }, Keys(profile));
            Assert.Equal("Excluir", PlanAdvice.Actions(profile).Single().Label);
            PlanNode angle = Node(NodeStatus.Ready, new ApiError(ErrorCodes.TemplateAngleDeviation, "desvío", "members[0]"));
            Assert.Equal(new[] { PlanAction.Exclude, PlanAction.Edit }, Keys(angle));

            PlanNode invalid = Node(NodeStatus.Invalid);
            invalid.Errors.Add(new ApiError(ErrorCodes.PlateOutsideGusset, "se sale", "members[1]"));
            Assert.Equal(new[] { PlanAction.Edit, PlanAction.Exclude }, Keys(invalid));

            PlanNode noMatch = Node(NodeStatus.NoMatch);
            noMatch.StatusDetail = "Ninguna plantilla casa todas sus ranuras con las barras del nudo.";
            Assert.Equal(new[] { PlanAction.Exclude, PlanAction.Template }, Keys(noMatch));
            PlanNode byDecision = Node(NodeStatus.NoMatch);
            byDecision.StatusDetail = "Sin plantilla por decisión de la persona (template: null).";
            Assert.Equal(new[] { PlanAction.Template }, Keys(byDecision));

            PlanNode missing = Node(NodeStatus.NoMatch, ChordMissing());
            missing.Members.Add(new PlanMember { ElementId = 1251056, AngleDeg = -135.6, Side = "-Y" });
            missing.Members.Add(new PlanMember { ElementId = 1251057, AngleDeg = -44.4, Side = "-Y" });
            Assert.Equal(new[] { PlanAction.Chord, PlanAction.Members }, Keys(missing));
            Assert.Equal("Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…", PlanAdvice.Advice(missing));

            Assert.Equal(new[] { PlanAction.Chord }, Keys(Node(NodeStatus.AmbiguousChord)));
            Assert.Equal(new[] { PlanAction.Show, PlanAction.Exclude }, Keys(Node(NodeStatus.Offset)));
            Assert.Equal(new[] { PlanAction.IncludeReplace }, Keys(Node(NodeStatus.AlreadyConnected)));
            Assert.Equal("Incluir (rehacer)", PlanAdvice.Actions(Node(NodeStatus.AlreadyConnected)).Single().Label);
            Assert.Equal(new[] { PlanAction.Include }, Keys(Node(NodeStatus.Excluded)));
            Assert.Empty(Keys(Node(NodeStatus.Untyped)));
            Assert.Equal(new[] { PlanAction.Show }, Keys(Node(NodeStatus.Created)));
            Assert.Equal(new[] { PlanAction.Show, PlanAction.Exclude }, Keys(Node(NodeStatus.Failed)));

            var emptyCatalog = new BatchPlan();
            PlanNode noTemplates = Node(NodeStatus.NoMatch);
            noTemplates.StatusDetail = "No hay plantillas en el catálogo (o ninguna de las pedidas existe).";
            Assert.Equal(new[] { PlanAction.Catalog }, Keys(noTemplates, emptyCatalog));
            Assert.Equal(new[] { PlanAction.Catalog }, Keys(noTemplates));
        }

        [Fact]
        public void Splice_SaysTheChordEndsHereAndNeverSendsToReselect()
        {
            // El empalme del cordón superior del Hangar (N9 en la 8c/8e): dos tramos de HSS12X8 terminan en el nudo y llegan
            // dos diagonales desde abajo. El detector toma un tramo como cordón (llega, no pasa de largo) y el otro queda
            // como barra del nudo a 0° o 180°.
            PlanNode splice = Node(NodeStatus.NoMatch, ChordMissing());
            splice.ChordElementId = 1245530;
            splice.Members.Add(new PlanMember { ElementId = 1245531, AngleDeg = 180.0, Side = "+Y", TypeName = "HSS12X8X1/2" });
            splice.Members.Add(new PlanMember { ElementId = 1251056, AngleDeg = -135.6, Side = "-Y" });
            splice.Members.Add(new PlanMember { ElementId = 1251057, AngleDeg = -44.4, Side = "-Y" });
            Assert.True(PlanAdvice.IsSplice(splice));
            Assert.Equal(2, PlanAdvice.DiagonalCount(splice));
            Assert.True(PlanAdvice.VisibleByDefault(splice));
            Assert.Equal("✖ Empalme del cordón", PlanAdvice.StatusText(splice));
            Assert.Equal(PlanAdvice.Red, PlanAdvice.ColorName(splice));
            Assert.Equal("El cordón termina en este nudo (empalme): ninguna plantilla encaja con 2 diagonales; crea esa típica o excluye", PlanAdvice.Advice(splice));
            Assert.DoesNotContain("selecciónalo y replanifica", PlanAdvice.Advice(splice));
            Assert.Equal(new[] { PlanAction.Exclude }, Keys(splice));

            // El otro tramo a 0° (el cordón elegido a mano con Cordón… es el que sale hacia −X): lo mismo.
            splice.Members[0] = new PlanMember { ElementId = 1245531, AngleDeg = 0.0, Side = "+Y" };
            Assert.True(PlanAdvice.IsSplice(splice));
            splice.Members[0] = new PlanMember { ElementId = 1245531, AngleDeg = 4.9, Side = "+Y" };
            Assert.True(PlanAdvice.IsSplice(splice));
            // Con solo diagonales (ningún tramo sigue) es el caso de la Fase 8: falta el cordón en la selección.
            splice.Members.RemoveAt(0);
            Assert.False(PlanAdvice.IsSplice(splice));
            Assert.Equal("✖ Falta el cordón", PlanAdvice.StatusText(splice));
            Assert.StartsWith("Falta el cordón en la selección", PlanAdvice.Advice(splice));

            var plan = new BatchPlan { Templates = { ["t"] = "Nudo tipico Detalle D" } };
            PlanNode other = Node(NodeStatus.NoMatch, ChordMissing());
            other.Name = "N16";
            other.Members.Add(new PlanMember { ElementId = 1, AngleDeg = -180.0, Side = "-Y" });
            other.Members.Add(new PlanMember { ElementId = 2, AngleDeg = -135.0, Side = "-Y" });
            plan.Nodes.Add(splice);
            plan.Nodes.Add(other);
            Assert.Equal("Ningún nudo listo: falta el cordón en 1 nudo. 1 empalme del cordón (sin plantilla).", PlanAdvice.SummaryText(plan));
        }

        [Fact]
        public void Splice_IsDetectedOnTwoChordSegmentsThatEndAtTheNode()
        {
            // Geometría: el cordón superior en dos tramos que terminan en X = 5000 y dos diagonales que llegan desde abajo.
            // El detector no corta ejes casi paralelos (ronda 8b), pero los extremos coinciden y se agrupan: un nudo de
            // cuatro barras sin ninguna que atraviese. Con el cordón fijado a mano al otro tramo pasa lo mismo.
            double z = 19933.0;
            const string chordType = "HSS12X8X1/2";
            var bars = new List<DetectorBar>
            {
                new DetectorBar(1, SyntheticTruss.P(0, z), SyntheticTruss.P(5000, z), chordType, 304.8),
                new DetectorBar(2, SyntheticTruss.P(5000, z), SyntheticTruss.P(10000, z), chordType, 304.8),
                SyntheticTruss.Bar(3, SyntheticTruss.P(5000, z), SyntheticTruss.P(5000 - 2500, z - 2500), SyntheticTruss.DiagonalType),
                SyntheticTruss.Bar(4, SyntheticTruss.P(5000, z), SyntheticTruss.P(5000 + 2500, z - 2500), SyntheticTruss.DiagonalType),
            };
            List<DetectedNode> nodes = NodeDetector.Detect(bars);
            DetectedNode splice = nodes.Single(n => n.MemberElementIds.Count >= 2);
            Assert.Equal(NodeStatus.Detected, splice.Status);
            Assert.False(splice.ChordContinuous);
            Assert.Contains(splice.ChordElementId, new long[] { 1, 2 });
            Assert.Equal(3, splice.Members.Count);
            DetectedMember segment = splice.Members.Single(m => m.ElementId == 1 || m.ElementId == 2);
            // El otro tramo sale a 0° (si el cordón elegido es el que llega desde −X) o a 180° (si es el que llega desde +X).
            Assert.True(System.Math.Abs(segment.AngleDeg) <= 5.0 || System.Math.Abs(segment.AngleDeg) >= 175.0, "ángulo del tramo: " + segment.AngleDeg);

            var facts = new SyntheticTrussFacts(bars);
            var request = new PlanRequest(facts, BatchPlanTests.ValidatorFor(facts, BatchPlanTests.Limits()))
            {
                SelectionIds = bars.Select(b => b.ElementId).ToList(),
                Templates = new List<MotorConexiones.Core.Catalog.CatalogTemplate> { BatchPlanTests.OfficialDetalleDTemplate() },
                DocumentTitle = "Empalme",
            };
            BatchPlan plan = PlanBuilder.Build(request);
            PlanNode node = plan.Nodes.Single(n => n.MemberElementIds.Count >= 2);
            Assert.Equal(NodeStatus.NoMatch, node.Status);
            Assert.True(PlanAdvice.ChordMissing(node));
            Assert.True(PlanAdvice.IsSplice(node));
            Assert.Equal("✖ Empalme del cordón", PlanAdvice.StatusText(node, plan));
            Assert.Equal("El cordón termina en este nudo (empalme): ninguna plantilla encaja con 2 diagonales; crea esa típica o excluye", PlanAdvice.Advice(node, plan));

            // Cordón fijado a mano al otro tramo: sigue siendo el empalme, con el mismo consejo.
            long otherSegment = node.ChordElementId == 1 ? 2 : 1;
            request.Overrides = BatchPlanTests.Overrides("{\"chord\": {\"" + node.Name + "\": " + otherSegment + "}}");
            request.PlanId = plan.PlanId;
            BatchPlan replanned = PlanBuilder.Build(request);
            PlanNode fixedChord = replanned.Find(node.Name)!;
            Assert.Equal(otherSegment, fixedChord.ChordElementId);
            Assert.False(fixedChord.ChordContinuous);
            Assert.True(PlanAdvice.IsSplice(fixedChord));
            Assert.StartsWith("El cordón termina en este nudo (empalme)", PlanAdvice.Advice(fixedChord, replanned));
            Assert.Equal(new[] { PlanAction.Exclude }, Keys(fixedChord, replanned));
            Assert.StartsWith("Ningún nudo listo: 1 empalme del cordón sin plantilla.", PlanAdvice.SummaryText(replanned));
        }

        [Fact]
        public void CreatedAndFailed_HaveTheirOwnTextsColorsAndHeader()
        {
            var plan = new BatchPlan { Templates = { ["t"] = "Nudo tipico Detalle D" } };
            PlanNode created = Node(NodeStatus.Created);
            created.Name = "N4";
            created.CreatedConnectionId = "0d39233d-4ef8-46ae-bd25-b1d49f0da6de";
            created.TemplateName = "Nudo tipico Detalle D";
            created.Orientation = "same";
            PlanNode createdWarned = Node(NodeStatus.Created, new ApiError(ErrorCodes.TemplateProfileDiffers, "El perfil de el cordón es 'HSS4X4' y la plantilla esperaba 'HSS3X3'.", "chord.profile"));
            createdWarned.Name = "N7";
            createdWarned.CreatedConnectionId = "conn-n7";
            PlanNode failed = Node(NodeStatus.Failed);
            failed.Name = "N11";
            failed.Errors.Add(new ApiError(ErrorCodes.FabricationFailed, "Fallo al modelar N11: la placa no se pudo escribir.", "batch:nodes[N11]"));
            failed.ValidationToken = new string('a', 64);
            failed.Spec = new System.Text.Json.Nodes.JsonObject();
            PlanNode ready = Node(NodeStatus.Ready);
            ready.Name = "N14";
            ready.ValidationToken = new string('b', 64);
            ready.Spec = new System.Text.Json.Nodes.JsonObject();
            plan.Nodes.AddRange(new[] { created, createdWarned, failed, ready });

            Assert.Equal("✔ Creada", PlanAdvice.StatusText(created));
            Assert.Equal("✔", PlanAdvice.Icon(created));
            Assert.Equal(PlanAdvice.Green, PlanAdvice.ColorName(created));
            Assert.Equal("Creada: conexión 0d39233d… · Ver en Revit; Borrar el lote la quita", PlanAdvice.Advice(created));
            Assert.Equal("N4 · Creada · Nudo tipico Detalle D · igual", PlanAdvice.MapLabel(created));
            Assert.Equal("✔ Creada con aviso", PlanAdvice.StatusText(createdWarned));
            Assert.Equal(PlanAdvice.Amber, PlanAdvice.ColorName(createdWarned));
            Assert.Equal("✖ Falló al crear", PlanAdvice.StatusText(failed));
            Assert.Equal(PlanAdvice.Red, PlanAdvice.ColorName(failed));
            Assert.Equal("Falló al crear (FABRICATION_FAILED: Fallo al modelar N11: la placa no se pudo escribir): corrige y pulsa Crear otra vez (solo crea los que faltan), o excluye", PlanAdvice.Advice(failed));
            Assert.True(BatchRunner.CanBeCreated(failed));
            Assert.True(BatchRunner.CanBeCreated(ready));
            Assert.False(BatchRunner.CanBeCreated(created));
            Assert.Equal("ya creada en este lote (conexión 0d39233d-4ef8-46ae-bd25-b1d49f0da6de)", BatchRunner.WhyNotCreatable(created));
            Assert.Equal(2, plan.CreatableCount);
            Assert.Equal("Crear 2 conexiones (1 reintento)", PlanAdvice.CreateButtonText(plan));
            Assert.Equal("Creadas 2 conexiones (1 con aviso), 1 falló. Quedan 1 lista sin crear.", PlanAdvice.SummaryText(plan));
            PlanAdvice.ApplyStatusColors(plan);
            Assert.Equal(new[] { "verde", "ambar", "rojo", "verde" }, plan.Nodes.Select(n => n.ColorName).ToArray());
            Assert.Contains("creada", PlanAdvice.Legend);
        }
    }
}
