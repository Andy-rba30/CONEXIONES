using System;

namespace MotorConexiones.Core.Units
{
    /// <summary>
    /// ÚNICO lugar del repositorio donde se convierten unidades (regla no negociable del encargo).
    /// El contrato usa milímetros y grados; Revit usa pies y radianes internamente.
    /// </summary>
    public static class UnitConverter
    {
        /// <summary>Un pie internacional son exactamente 304,8 mm.</summary>
        public const double MillimetersPerFoot = 304.8;

        public static double MmToFeet(double millimeters) => millimeters / MillimetersPerFoot;

        public static double FeetToMm(double feet) => feet * MillimetersPerFoot;

        public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

        public static double RadiansToDegrees(double radians) => radians * 180.0 / Math.PI;

        /// <summary>Redondea a la décima de milímetro (es la precisión con la que se firma el <c>validation_token</c>).</summary>
        public static double RoundMm(double millimeters) => Math.Round(millimeters, 1, MidpointRounding.AwayFromZero);
    }
}
