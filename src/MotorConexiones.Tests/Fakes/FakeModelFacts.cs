using System.Collections.Generic;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;

namespace MotorConexiones.Tests.Fakes
{
    /// <summary>
    /// Implementación simulada de IModelFacts con los datos del nudo real del Hangar
    /// registrados en docs/fases/resultados-fase-1.md (cordón 1249510 y miembros 1249630, 1249631, 1249636).
    /// Los ángulos van con signo en el marco canónico (Fase 7): cordón hacia +X, +Y hacia arriba.
    /// </summary>
    public sealed class FakeModelFacts : IModelFacts
    {
        public string ProjectUniqueId { get; set; } = "b89f2a41-e794-4d87-9bc1-872fba7d0124-00045a1f";

        public Dictionary<long, MemberModelFacts> Members { get; } = new Dictionary<long, MemberModelFacts>();
        public HashSet<long> NonMemberElements { get; } = new HashSet<long>();
        public List<string> AvailableProfiles { get; } = new List<string>();

        public FakeModelFacts()
        {
            // Cordón HSS3X3X1/4
            Members[1249510] = new MemberModelFacts
            {
                ElementId = 1249510,
                UniqueId = "b89f2a41-e794-4d87-9bc1-872fba7d0124-00131106",
                FamilyName = "HSS-Square",
                TypeName = "HSS3X3X1/4",
                WidthMm = 76.2,
                HeightMm = 76.2,
                ThicknessMm = 6.35,
                CurveStartMm = new Vec3(-15000.0, -17195.8, 17423.0),
                CurveEndMm = new Vec3(-8000.0, -17195.8, 17423.0),
                AngleInPlaneDeg = 0.0,
                ConnectsToNode = true
            };

            // Diagonal superior 1249630
            Members[1249630] = new MemberModelFacts
            {
                ElementId = 1249630,
                UniqueId = "b89f2a41-e794-4d87-9bc1-872fba7d0124-0013117e",
                FamilyName = "HSS-Square-64x64",
                TypeName = "HSS2-1-2X2-1-2X3-16 64x64",
                WidthMm = 63.5,
                HeightMm = 63.5,
                ThicknessMm = 4.76,
                CurveStartMm = new Vec3(-11867.7, -17195.8, 17423.0),
                CurveEndMm = new Vec3(-10453.5, -17195.8, 18837.2),
                AngleInPlaneDeg = 45.0,
                ConnectsToNode = true
            };

            // Montante vertical 1249631
            Members[1249631] = new MemberModelFacts
            {
                ElementId = 1249631,
                UniqueId = "b89f2a41-e794-4d87-9bc1-872fba7d0124-0013117f",
                FamilyName = "HSS-Square-64x64",
                TypeName = "HSS2-1-2X2-1-2X3-16 64x64",
                WidthMm = 63.5,
                HeightMm = 63.5,
                ThicknessMm = 4.76,
                CurveStartMm = new Vec3(-11867.7, -17195.8, 17423.0),
                CurveEndMm = new Vec3(-11867.7, -17195.8, 19423.0),
                AngleInPlaneDeg = 90.0,
                ConnectsToNode = true
            };

            // Diagonal inferior 1249636
            Members[1249636] = new MemberModelFacts
            {
                ElementId = 1249636,
                UniqueId = "b89f2a41-e794-4d87-9bc1-872fba7d0124-00131184",
                FamilyName = "HSS-Square-64x64",
                TypeName = "HSS2-1-2X2-1-2X3-16 64x64",
                WidthMm = 63.5,
                HeightMm = 63.5,
                ThicknessMm = 4.76,
                CurveStartMm = new Vec3(-11867.7, -17195.8, 17423.0),
                CurveEndMm = new Vec3(-13281.9, -17195.8, 16008.8),
                AngleInPlaneDeg = -135.0,
                ConnectsToNode = true
            };

            AvailableProfiles.AddRange(new[]
            {
                "HSS3X3X1/4",
                "HSS2-1-2X2-1-2X3-16 64x64",
                "HSS2-1/2X2-1/2X3/16",
                "HSS4X4X3/8",
                "HSS5X5X1/2",
                "W12X26",
                "W14X90"
            });
        }

        public bool ElementExists(long elementId)
        {
            return Members.ContainsKey(elementId) || NonMemberElements.Contains(elementId);
        }

        public bool IsStructuralMember(long elementId)
        {
            return Members.ContainsKey(elementId);
        }

        public MemberModelFacts? GetMemberFacts(long elementId)
        {
            return Members.TryGetValue(elementId, out var facts) ? facts : null;
        }

        public IReadOnlyList<string> GetAvailableProfileNames()
        {
            return AvailableProfiles;
        }

        public bool CheckClashWithForeignMember(long foreignMemberId, Vec3 startMm, Vec3 endMm)
        {
            return false;
        }
    }
}
