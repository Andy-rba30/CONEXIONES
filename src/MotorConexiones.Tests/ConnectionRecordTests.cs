using System.Collections.Generic;
using MotorConexiones.Core.Storage;
using Xunit;

namespace MotorConexiones.Tests
{
    public class ConnectionRecordTests
    {
        [Fact]
        public void RoundTripSerializationPreservesAllFields()
        {
            var original = new ConnectionRecord
            {
                ConnectionId = "test-uuid-1234",
                SpecVersion = "1.0",
                ConnectionType = "gusset_node",
                SpecJson = "{\"test\":true}",
                CreatedElementIds = new List<long> { 101, 102, 103 },
                CreatedUtc = "2026-10-01T00:00:00.0000000Z",
                ModifiedMembers = new List<ModifiedMemberRecord>
                {
                    new ModifiedMemberRecord
                    {
                        ElementId = 1249630,
                        EndIndex = 0,
                        ParameterName = "START_EXTENSION",
                        OriginalValueFeet = 0.0,
                        AppliedSetbackMm = 180.0
                    },
                    new ModifiedMemberRecord
                    {
                        ElementId = 1249631,
                        EndIndex = 1,
                        ParameterName = "END_EXTENSION",
                        OriginalValueFeet = -0.5,
                        AppliedSetbackMm = 150.0
                    }
                }
            };

            string json = original.ToJson();
            Assert.False(string.IsNullOrWhiteSpace(json));

            var deserialized = ConnectionRecord.FromJson(json);
            Assert.NotNull(deserialized);
            Assert.Equal("test-uuid-1234", deserialized!.ConnectionId);
            Assert.Equal("1.0", deserialized.SpecVersion);
            Assert.Equal("gusset_node", deserialized.ConnectionType);
            Assert.Equal("{\"test\":true}", deserialized.SpecJson);
            Assert.Equal(new List<long> { 101, 102, 103 }, deserialized.CreatedElementIds);
            Assert.Equal(2, deserialized.ModifiedMembers.Count);

            var m1 = deserialized.ModifiedMembers[0];
            Assert.Equal(1249630, m1.ElementId);
            Assert.Equal(0, m1.EndIndex);
            Assert.Equal("START_EXTENSION", m1.ParameterName);
            Assert.Equal(0.0, m1.OriginalValueFeet);
            Assert.Equal(180.0, m1.AppliedSetbackMm);

            var m2 = deserialized.ModifiedMembers[1];
            Assert.Equal(1249631, m2.ElementId);
            Assert.Equal(1, m2.EndIndex);
            Assert.Equal("END_EXTENSION", m2.ParameterName);
            Assert.Equal(-0.5, m2.OriginalValueFeet);
            Assert.Equal(150.0, m2.AppliedSetbackMm);
        }

        [Fact]
        public void FromJson_ReturnsNullOnEmpty()
        {
            Assert.Null(ConnectionRecord.FromJson(string.Empty));
            Assert.Null(ConnectionRecord.FromJson("   "));
        }
    }
}
