using System.Collections.Generic;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Core.Model
{
    /// <summary>
    /// Interfaz que desacopla el Core de la API de Revit para consultar hechos del modelo necesarios en la validación
    /// (existencia de elementos, tipos de perfil, ángulos reales, colisiones) y para la firma del validation_token.
    /// La Fase 3 la implementará con Revit; las pruebas usan una implementación simulada.
    /// </summary>
    public interface IModelFacts
    {
        /// <summary>ProjectInformation.UniqueId del documento activo en Revit.</summary>
        string ProjectUniqueId { get; }

        /// <summary>Comprueba si existe un elemento con el ElementId indicado.</summary>
        bool ElementExists(long elementId);

        /// <summary>Comprueba si el elemento pertenece a la categoría de armazón estructural (Structural Framing).</summary>
        bool IsStructuralMember(long elementId);

        /// <summary>Obtiene los datos geométricos y de perfil del miembro especificado.</summary>
        MemberModelFacts? GetMemberFacts(long elementId);

        /// <summary>Lista de nombres de tipos de perfil cargados en el modelo para sugerencias en PROFILE_MISMATCH.</summary>
        IReadOnlyList<string> GetAvailableProfileNames();

        /// <summary>Comprueba si hay interferencia o choque con barras que no forman parte de la unión.</summary>
        bool CheckClashWithForeignMember(long foreignMemberId, Vec3 startMm, Vec3 endMm);
    }

    /// <summary>
    /// Hechos del modelo para una barra de armazón estructural.
    /// </summary>
    public sealed class MemberModelFacts
    {
        public long ElementId { get; set; }
        public string UniqueId { get; set; } = string.Empty;
        public string FamilyName { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;

        public double WidthMm { get; set; }
        public double HeightMm { get; set; }
        public double ThicknessMm { get; set; }

        public Vec3 CurveStartMm { get; set; }
        public Vec3 CurveEndMm { get; set; }

        /// <summary>Ángulo de la barra medido en el plano de la cercha en grados (ej. 45° o 90°).</summary>
        public double AngleInPlaneDeg { get; set; }

        /// <summary>Indica si el miembro llega físicamente al nudo (ejes intersectan dentro de la tolerancia).</summary>
        public bool ConnectsToNode { get; set; } = true;
    }
}
