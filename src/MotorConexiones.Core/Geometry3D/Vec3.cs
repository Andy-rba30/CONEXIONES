using System;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>Vector o punto 3D en milímetros. Sin dependencia de Revit para poder probarlo en la nube.</summary>
    public readonly struct Vec3 : IEquatable<Vec3>
    {
        public Vec3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public static readonly Vec3 Zero = new Vec3(0, 0, 0);
        public static readonly Vec3 UnitX = new Vec3(1, 0, 0);
        public static readonly Vec3 UnitY = new Vec3(0, 1, 0);
        public static readonly Vec3 UnitZ = new Vec3(0, 0, 1);

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        public static Vec3 operator +(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vec3 operator -(Vec3 a, Vec3 b) => new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vec3 operator -(Vec3 a) => new Vec3(-a.X, -a.Y, -a.Z);
        public static Vec3 operator *(Vec3 a, double k) => new Vec3(a.X * k, a.Y * k, a.Z * k);
        public static Vec3 operator *(double k, Vec3 a) => a * k;

        public double Dot(Vec3 other) => X * other.X + Y * other.Y + Z * other.Z;

        public Vec3 Cross(Vec3 other) => new Vec3(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X);

        public Vec3 Normalized()
        {
            double length = Length;
            if (length < 1e-12) throw new InvalidOperationException("No se puede normalizar un vector de longitud cero.");
            return new Vec3(X / length, Y / length, Z / length);
        }

        public double DistanceTo(Vec3 other) => (this - other).Length;

        public bool Equals(Vec3 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        public override bool Equals(object? obj) => obj is Vec3 other && Equals(other);
        public override int GetHashCode() => unchecked((X.GetHashCode() * 397 ^ Y.GetHashCode()) * 397 ^ Z.GetHashCode());
        public override string ToString() => string.Format(System.Globalization.CultureInfo.InvariantCulture, "({0:0.###}, {1:0.###}, {2:0.###})", X, Y, Z);
    }
}
