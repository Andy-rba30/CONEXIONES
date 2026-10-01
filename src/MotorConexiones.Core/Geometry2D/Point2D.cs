using System;

namespace MotorConexiones.Core.Geometry2D
{
    /// <summary>
    /// Punto o vector 2D en milímetros en el plano local del nudo.
    /// </summary>
    public readonly struct Point2D : IEquatable<Point2D>
    {
        public double X { get; }
        public double Y { get; }

        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(Point2D other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public Point2D Add(Point2D other) => new Point2D(X + other.X, Y + other.Y);
        public Point2D Subtract(Point2D other) => new Point2D(X - other.X, Y - other.Y);
        public Point2D Scale(double factor) => new Point2D(X * factor, Y * factor);

        public static Point2D operator +(Point2D a, Point2D b) => a.Add(b);
        public static Point2D operator -(Point2D a, Point2D b) => a.Subtract(b);
        public static Point2D operator *(Point2D a, double s) => a.Scale(s);
        public static Point2D operator *(double s, Point2D a) => a.Scale(s);

        public bool Equals(Point2D other)
        {
            return Math.Abs(X - other.X) < 1e-7 && Math.Abs(Y - other.Y) < 1e-7;
        }

        public override bool Equals(object? obj) => obj is Point2D other && Equals(other);
        public override int GetHashCode() => unchecked((X.GetHashCode() * 397) ^ Y.GetHashCode());
        public override string ToString() => $"({X:F2}, {Y:F2}) mm";

        public static bool operator ==(Point2D left, Point2D right) => left.Equals(right);
        public static bool operator !=(Point2D left, Point2D right) => !left.Equals(right);
    }
}
