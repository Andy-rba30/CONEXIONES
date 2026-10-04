using System;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Sistema de coordenadas local del nudo. Desde la Fase 7 es <b>canónico</b> (decisión P5 de la propuesta de
    /// catálogo): el mismo nudo da el mismo marco aunque el cordón esté dibujado al revés, así las plantillas del
    /// catálogo se pueden comparar entre nudos.
    /// <list type="bullet">
    /// <item>X = eje del cordón orientado hacia +X global (si el cordón va en Y, hacia +Y global; si es vertical, hacia +Z).</item>
    /// <item>Y = en el plano de la cercha (cordón + primer miembro), orientado hacia +Z global: en una cercha vertical
    /// +Y apunta hacia arriba; en una cercha horizontal, hacia +Y global.</item>
    /// <item>Z = X × Y (normal al plano de la cercha).</item>
    /// </list>
    /// Origen = punto de trabajo = punto medio del segmento más corto entre los ejes del cordón y del primer miembro
    /// (sección 7 del encargo; esa regla y la de los 5 mm no cambian). Todo en milímetros.
    /// </summary>
    public sealed class NodeFrame
    {
        /// <summary>Distancia máxima admitida entre los ejes del cordón y del primer miembro.</summary>
        public const double MaxAxisDistanceMm = 5.0;

        /// <summary>
        /// Por debajo de este valor una componente global se considera cero al elegir el sentido de los ejes. 1e-4
        /// equivale a 0,006°: el modelo real trae ruido de 1e-6 en los ejes (resultados de la Fase 1), que con una
        /// tolerancia más fina invertía la normal.
        /// </summary>
        public const double VerticalComponentTolerance = 1e-4;

        private NodeFrame(Vec3 origin, Vec3 x, Vec3 y, Vec3 z, double axisDistanceMm, bool chordReversed)
        {
            Origin = origin;
            X = x;
            Y = y;
            Z = z;
            AxisDistanceMm = axisDistanceMm;
            ChordReversed = chordReversed;
        }

        public Vec3 Origin { get; }
        public Vec3 X { get; }
        public Vec3 Y { get; }
        public Vec3 Z { get; }

        /// <summary>Distancia medida entre los ejes (0 si se cortan exactamente).</summary>
        public double AxisDistanceMm { get; }

        /// <summary>
        /// Verdadero si la curva de ubicación del cordón va en sentido contrario a +X local (el cordón está dibujado
        /// "al revés" respecto al marco canónico). Solo informativo: no cambia nada de lo que se crea.
        /// </summary>
        public bool ChordReversed { get; }

        /// <summary>Convierte coordenadas locales (mm) a globales (mm).</summary>
        public Vec3 ToGlobal(double x, double y, double z) => Origin + X * x + Y * y + Z * z;

        /// <summary>Convierte un punto global (mm) a coordenadas locales (mm).</summary>
        public Vec3 ToLocal(Vec3 global)
        {
            Vec3 d = global - Origin;
            return new Vec3(d.Dot(X), d.Dot(Y), d.Dot(Z));
        }

        /// <summary>
        /// Calcula el sistema local canónico a partir de los ejes del cordón y del primer miembro (rectas infinitas).
        /// Lanza <see cref="NodeGeometryException"/> con código <see cref="ErrorCodes.NodeAxesNotIntersecting"/>
        /// si los ejes distan más de 5 mm, o <see cref="ErrorCodes.NodeAxesParallel"/> si son paralelos.
        /// </summary>
        public static NodeFrame Compute(Vec3 chordStart, Vec3 chordEnd, Vec3 memberStart, Vec3 memberEnd)
        {
            Vec3 chordDirection = (chordEnd - chordStart).Normalized();
            Vec3 d = (memberEnd - memberStart).Normalized();

            Vec3 cross = chordDirection.Cross(d);
            if (cross.Length < 1e-6)
            {
                throw new NodeGeometryException(
                    ErrorCodes.NodeAxesParallel,
                    "El eje del cordón y el del primer miembro son paralelos: no definen el plano de la cercha.",
                    "Elige como primer miembro una diagonal o un montante que llegue al nudo.");
            }

            // Segmento más corto entre dos rectas: p = chordStart + x*s, q = memberStart + d*t.
            Vec3 w = chordStart - memberStart;
            double b = chordDirection.Dot(d);
            double dd = chordDirection.Dot(w);
            double e = d.Dot(w);
            double denominator = 1.0 - b * b; // = |x × d|², > 0 porque no son paralelas
            double s = (b * e - dd) / denominator;
            double t = (e - b * dd) / denominator;
            Vec3 p = chordStart + chordDirection * s;
            Vec3 q = memberStart + d * t;
            double distance = p.DistanceTo(q);

            if (distance > MaxAxisDistanceMm)
            {
                throw new NodeGeometryException(
                    ErrorCodes.NodeAxesNotIntersecting,
                    string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "El eje del cordón y el del primer miembro no se cortan: distan {0:0.0} mm (máximo {1:0.0} mm).",
                        distance, MaxAxisDistanceMm),
                    "Comprueba en el modelo que las líneas de ubicación de los miembros llegan al eje del cordón.");
            }

            Vec3 origin = (p + q) * 0.5;

            // X canónico: el eje del cordón hacia +X global (o +Y, o +Z), independiente de cómo esté dibujado.
            bool reversed = !PointsPositive(chordDirection);
            Vec3 x = reversed ? -chordDirection : chordDirection;

            // Y canónico: dentro del plano de la cercha y perpendicular a X, hacia +Z global (hacia arriba).
            Vec3 normal = cross.Normalized();
            Vec3 y = normal.Cross(x).Normalized();
            if (!PointsUp(y)) y = -y;

            Vec3 z = x.Cross(y).Normalized();
            return new NodeFrame(origin, x, y, z, distance, reversed);
        }

        /// <summary>Sentido canónico de X: componente global X positiva; si es ~0, Y positiva; si también, Z positiva.</summary>
        private static bool PointsPositive(Vec3 v)
        {
            if (Math.Abs(v.X) > VerticalComponentTolerance) return v.X > 0;
            if (Math.Abs(v.Y) > VerticalComponentTolerance) return v.Y > 0;
            return v.Z >= 0;
        }

        /// <summary>Sentido canónico de Y: componente global Z positiva (arriba); si es ~0, Y positiva; si también, X positiva.</summary>
        private static bool PointsUp(Vec3 v)
        {
            if (Math.Abs(v.Z) > VerticalComponentTolerance) return v.Z > 0;
            if (Math.Abs(v.Y) > VerticalComponentTolerance) return v.Y > 0;
            return v.X >= 0;
        }

        // ---- ángulos en el plano local (Fase 7: con signo) ----

        /// <summary>
        /// Ángulo con signo de una dirección (ux, uy) del plano local, medido desde +X en grados, en [−180°, 180°):
        /// +45° = hacia +X y +Y (arriba a la derecha); −135° = hacia −X y −Y (abajo a la izquierda).
        /// </summary>
        public static double SignedAngleDeg(double ux, double uy) => NormalizeDeg(UnitConverter.RadiansToDegrees(Math.Atan2(uy, ux)));

        /// <summary>Lleva un ángulo en grados al intervalo [−180°, 180°).</summary>
        public static double NormalizeDeg(double degrees)
        {
            double a = degrees % 360.0;
            if (a < -180.0) a += 360.0;
            if (a >= 180.0) a -= 360.0;
            return a;
        }

        /// <summary>
        /// Inclinación respecto al eje del cordón, sin signo y sin distinguir el sentido a lo largo del cordón, en
        /// [0°, 90°]: es lo que escribe un plano (45° = 135° = −45° = −135°).
        /// </summary>
        public static double AngleToChordDeg(double degrees)
        {
            double a = Math.Abs(NormalizeDeg(degrees));
            return a > 90.0 ? 180.0 - a : a;
        }

        /// <summary>Diferencia absoluta entre dos ángulos con signo, teniendo en cuenta la vuelta completa (0..180).</summary>
        public static double AngleDifferenceDeg(double a, double b) => Math.Abs(NormalizeDeg(a - b));

        /// <summary>Lado del cordón al que apunta una dirección del plano local: <c>+Y</c> o <c>-Y</c>.</summary>
        public static string SideOf(double uy) => uy >= 0 ? "+Y" : "-Y";
    }

    /// <summary>Error geométrico del nudo con código del contrato.</summary>
    public sealed class NodeGeometryException : Exception
    {
        public NodeGeometryException(string code, string message, string? hint = null) : base(message)
        {
            Code = code;
            Hint = hint;
        }

        public string Code { get; }
        public string? Hint { get; }
    }
}
