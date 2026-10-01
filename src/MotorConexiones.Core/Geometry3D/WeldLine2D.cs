namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>Línea de soldadura de filete en el plano del nudo (mm, sistema local del nudo).</summary>
    public readonly struct WeldLine2D
    {
        public WeldLine2D(BoltPosition start, BoltPosition end, double sizeMm)
        {
            Start = start;
            End = end;
            SizeMm = sizeMm;
        }

        public BoltPosition Start { get; }
        public BoltPosition End { get; }
        public double SizeMm { get; }
    }
}
