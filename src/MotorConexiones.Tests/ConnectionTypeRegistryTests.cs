using MotorConexiones.Core.Types;
using Xunit;

namespace MotorConexiones.Tests
{
    [Collection("ConnectionTypeRegistry")] // el registro es estático: estas pruebas no corren en paralelo con SketchBuilderTests
    public class ConnectionTypeRegistryTests
    {
        private sealed class FakeType : IConnectionType
        {
            public FakeType(string name) { Name = name; }
            public string Name { get; }
            public string Description => "prueba";
            public string GetSchemaJson() => "{}";
            public string GetExampleJson() => "{}";
        }

        [Fact]
        public void RegisterFindAndList()
        {
            ConnectionTypeRegistry.Clear();
            ConnectionTypeRegistry.Register(new FakeType("gusset_node"));
            ConnectionTypeRegistry.Register(new FakeType("base_plate"));

            Assert.NotNull(ConnectionTypeRegistry.Find("gusset_node"));
            Assert.Null(ConnectionTypeRegistry.Find("no_existe"));
            Assert.Equal(new[] { "base_plate", "gusset_node" }, System.Linq.Enumerable.Select(ConnectionTypeRegistry.All, t => t.Name));
            ConnectionTypeRegistry.Clear();
        }
    }
}
