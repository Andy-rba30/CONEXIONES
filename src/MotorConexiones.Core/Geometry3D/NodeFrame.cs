using System;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Sistema de coordenadas local del nudo (decisión cerrada, sección 7 del encargo):
    /// X = eje del cordón (inicio → fin); Z = normal al plano de la cercha = X × d (d = dirección del primer miembro),
    /// con el signo elegido para que Z tenga componente global Z positiva (si es cero, componente Y positiva);
    /// Y = Z × X. Origen = punto de trabajo = punto medio del segmento más corto entre los dos ejes.
    /// Todo en milímetros.
    /// </summary>
    public sealed class NodeFrame
    {
        /// <summary>Distancia máxima admitida entre los ejes del cordón y del primer miembro.</summary>
        public const double MaxAxisDistanceMm = 5.0;

        /// <summary>
        /// Por debajo de este valor la componente global Z de la normal se considera cero (cercha vertical) y el signo
        /// se decide por la componente Y. 1e-4 equivale a 0,006°: el modelo real trae ruido de 1e-6 en los ejes
        /// (resultados de la Fase 1), que con una tolerancia más fina invertía la normal.
        /// </summary>
        public const double VerticalComponentTolerance = 1e-4;

        private NodeFrame(Vec3 origin, Vec3 x, Vec3 y, Vec3 z, double axisDistanceMm)
        {
            Origin = origin;
            X = x;
            Y = y;
            Z = z;
            AxisDistanceMm = axisDistanceMm;
        }

        public Vec3 Origin { get; }
        public Vec3 X { get; }
        public Vec3 Y { get; }
        public Vec3 Z { get; }

        /// <summary>Distancia medida entre los ejes (0 si se cortan exactamente).</summary>
        public double AxisDistanceMm { get; }

        /// <summary>Convierte coordenadas locales (mm) a globales (mm).</summary>
        public Vec3 ToGlobal(double x, double y, double z) => Origin + X * x + Y * y + Z * z;

        /// <summary>Convierte un punto global (mm) a coordenadas locales (mm).</summary>
        public Vec3 ToLocal(Vec3 global)
        {
            Vec3 d = global - Origin;
            return new Vec3(d.Dot(X), d.Dot(Y), d.Dot(Z));
        }

        /// <summary>
        /// Calcula el sistema local a partir de los ejes del cordón y del primer miembro (rectas infinitas).
        /// Lanza <see cref="NodeGeometryException"/> con código <see cref="ErrorCodes.NodeAxesNotIntersecting"/>
        /// si los ejes distan más de 5 mm, o <see cref="ErrorCodes.NodeAxesParallel"/> si son paralelos.
        /// </summary>
        public static NodeFrame Compute(Vec3 chordStart, Vec3 chordEnd, Vec3 memberStart, Vec3 memberEnd)
        {
            Vec3 x = (chordEnd - chordStart).Normalized();
            Vec3 d = (memberEnd - memberStart).Normalized();

            Vec3 cross = x.Cross(d);
            if (cross.Length < 1e-6)
            {
                throw new NodeGeometryException(
                    ErrorCodes.NodeAxesParallel,
                    "El eje del cordón y el del primer miembro son paralelos: no definen el plano de la cercha.",
                    "Elige como primer miembro una diagonal o un montante que llegue al nudo.");
            }

            // Segmento más corto entre dos rectas: p = chordStart + x*s, q = memberStart + d*t.
            Vec3 w = chordStart - memberStart;
            double b = x.Dot(d);
            double dd = x.Dot(w);
            double e = d.Dot(w);
            double denominator = 1.0 - b * b; // = |x × d|², > 0 porque no son paralelas
            double s = (b * e - dd) / denominator;
            double t = (e - b * dd) / denominator;
            Vec3 p = chordStart + x * s;
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

            Vec3 z = cross.Normalized();
            if (Math.Abs(z.Z) > VerticalComponentTolerance)
            {
                if (z.Z < 0) z = -z;
            }
            else if (z.Y < 0)
            {
                z = -z;
            }

            Vec3 y = z.Cross(x).Normalized();
            return new NodeFrame(origin, x, y, z, distance);
        }
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
