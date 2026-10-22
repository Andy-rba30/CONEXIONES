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

        /// <summary>
        /// Lado de la sección <b>en el plano de la cercha</b> (lo que se ve en el alzado: el medio canto del cordón que la
        /// barra cortada contra su cara necesita, la banda del croquis). Ronda 12c: en el cordón superior del Hangar es el 8"
        /// del HSS12X8X1/2 (203,2), porque el ejemplar está girado 90° (<see cref="SectionOrientation"/>).
        /// </summary>
        public double WidthMm { get; set; }

        /// <summary>
        /// Lado de la sección <b>perpendicular al plano</b> de la cercha (la cara del cordón a lo ancho; a lo ancho de la
        /// plancha de un empalme). En el Hangar, el 12" del HSS12X8X1/2 (304,8). Igual a <see cref="WidthMm"/> en un cuadrado.
        /// </summary>
        public double HeightMm { get; set; }

        public double ThicknessMm { get; set; }

        /// <summary>Ronda 12c: giro de la sección del ejemplar (Cross-Section Rotation) en grados; 0 si no se conoce.</summary>
        public double SectionRotationDeg { get; set; }

        public Vec3 CurveStartMm { get; set; }
        public Vec3 CurveEndMm { get; set; }

        /// <summary>
        /// Ángulo de la barra en el plano de la cercha, con signo, medido desde +X del marco canónico del nudo en
        /// [−180°, 180°) (Fase 7). Sin marco del nudo, la pendiente respecto a la horizontal.
        /// </summary>
        public double AngleInPlaneDeg { get; set; }

        /// <summary>Indica si el miembro llega físicamente al nudo (ejes intersectan dentro de la tolerancia).</summary>
        public bool ConnectsToNode { get; set; } = true;

        /// <summary>
        /// Fase 12 (opción A): dónde corta el eje de la barra al eje del cordón, medido desde el punto de trabajo a lo largo
        /// de +X del marco canónico, con signo (nulo sin marco). 0 cuando la barra concurre.
        /// </summary>
        public double? AxisOffsetMm { get; set; }

        /// <summary>Fase 12: distancia de la recta del eje de la barra al punto de trabajo (nulo sin marco).</summary>
        public double? AxisDistanceMm { get; set; }

        /// <summary>Fase 12: distancia del extremo de la barra más cercano al punto de trabajo (nulo sin marco).</summary>
        public double? EndGapMm { get; set; }
    }
}
