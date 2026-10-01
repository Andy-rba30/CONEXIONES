namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>Posición 2D de un punto o perno en el plano de la placa (mm, sistema local del nudo).</summary>
    public readonly struct BoltPosition
    {
        public BoltPosition(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; }
        public double Y { get; }

        public override string ToString() => string.Format(System.Globalization.CultureInfo.InvariantCulture, "({0:0.##}, {1:0.##})", X, Y);
    }
}
