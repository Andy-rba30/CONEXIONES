using System;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Paquete que atraviesan los pernos de una placa cuchilla: la cartela y la placa, solapadas cara con cara (ronda 6b).
    /// La cartela está centrada en el plano XY del sistema local (Z de −t/2 a +t/2); la placa cuchilla apoya sobre una de
    /// sus caras (<c>plate.gusset_face</c>: <c>+z</c> por defecto o <c>−z</c>), así que su plano medio queda desplazado
    /// ±(t_cartela + t_placa)/2 y el agarre de los pernos es exactamente t_cartela + t_placa. La longitud del perno sale
    /// de <c>bolts.length_mm</c> si el plano la trae; si no, del agarre más el suplemento de <c>config/limits.json</c>
    /// (<see cref="LimitsConfig.ComputeBoltLengthMm"/>). Antes de esta ronda la placa se creaba en el mismo plano que la
    /// cartela y los pernos medían 45 mm fijos, sin relación con los espesores.
    /// </summary>
    public sealed class BoltStack
    {
        public const string FacePositive = "+z";
        public const string FaceNegative = "-z";

        public BoltStack(double gussetThicknessMm, double plateThicknessMm, int side, double boltLengthMm, double lengthAdditionMm, bool lengthFromSpec)
        {
            if (gussetThicknessMm <= 0) throw new ArgumentOutOfRangeException(nameof(gussetThicknessMm), "El espesor de la cartela debe ser positivo.");
            if (plateThicknessMm <= 0) throw new ArgumentOutOfRangeException(nameof(plateThicknessMm), "El espesor de la placa cuchilla debe ser positivo.");
            GussetThicknessMm = gussetThicknessMm;
            PlateThicknessMm = plateThicknessMm;
            Side = side >= 0 ? 1 : -1;
            BoltLengthMm = boltLengthMm;
            LengthAdditionMm = lengthAdditionMm;
            LengthFromSpec = lengthFromSpec;
        }

        public double GussetThicknessMm { get; }
        public double PlateThicknessMm { get; }

        /// <summary>+1: la placa apoya en la cara +Z de la cartela; −1: en la cara −Z.</summary>
        public int Side { get; }

        /// <summary>Texto del contrato de la cara: "+z" o "-z".</summary>
        public string FaceLabel => Side > 0 ? FacePositive : FaceNegative;

        /// <summary>Espesor total que atraviesan los pernos: cartela + placa cuchilla.</summary>
        public double GripMm => GussetThicknessMm + PlateThicknessMm;

        /// <summary>Desplazamiento en Z del plano medio de la placa cuchilla respecto al plano medio de la cartela.</summary>
        public double PlateOffsetMm => Side * (GussetThicknessMm + PlateThicknessMm) / 2.0;

        /// <summary>Cara inferior del paquete (menor Z): cara −Z de la cartela o cara exterior de la placa.</summary>
        public double StackMinMm => Side > 0 ? -GussetThicknessMm / 2.0 : -(GussetThicknessMm / 2.0 + PlateThicknessMm);

        /// <summary>Cara superior del paquete (mayor Z).</summary>
        public double StackMaxMm => Side > 0 ? GussetThicknessMm / 2.0 + PlateThicknessMm : GussetThicknessMm / 2.0;

        /// <summary>Longitud del perno que se crea (bajo cabeza).</summary>
        public double BoltLengthMm { get; }

        /// <summary>Suplemento sobre el agarre (tuerca, arandela, rosca) que usa el cálculo o la comprobación.</summary>
        public double LengthAdditionMm { get; }

        /// <summary>Longitud mínima razonable: agarre + suplemento. Por debajo, el validador avisa.</summary>
        public double MinimumLengthMm => GripMm + LengthAdditionMm;

        /// <summary>Verdadero si la longitud vino de <c>bolts.length_mm</c>; falso si se calculó del agarre.</summary>
        public bool LengthFromSpec { get; }

        /// <summary>
        /// Lee <c>plate.gusset_face</c>: "+z" (o vacío) → +1, "-z" → −1. <paramref name="recognized"/> es falso con
        /// cualquier otro texto (el esquema lo marca como error; aquí se asume +z para no dejar de dibujar).
        /// </summary>
        public static int ParseSide(string? face, out bool recognized)
        {
            recognized = true;
            if (string.IsNullOrWhiteSpace(face)) return 1;
            string clean = face!.Trim().ToLowerInvariant();
            if (clean == FacePositive || clean == "z" || clean == "+") return 1;
            if (clean == FaceNegative || clean == "−z" || clean == "-") return -1;
            recognized = false;
            return 1;
        }

        /// <summary>Paquete de una placa cuchilla con los valores de la especificación y los límites configurados.</summary>
        public static BoltStack Compute(double gussetThicknessMm, KnifePlateSpec plate, BoltPatternSpec? bolts, LimitsConfig? limits)
        {
            if (plate == null) throw new ArgumentNullException(nameof(plate));
            limits ??= LimitsConfig.Default;
            double gusset = gussetThicknessMm > 0 ? gussetThicknessMm : 9.525;
            double plateThickness = plate.ThicknessMm.GetValueOrDefault(10.0);
            if (plateThickness <= 0) plateThickness = 10.0;
            int side = ParseSide(plate.GussetFace, out _);
            double diameter = bolts?.DiameterMm ?? 15.875;
            double addition = limits.GetBoltLengthAdditionMm(diameter);
            double grip = gusset + plateThickness;
            bool fromSpec = bolts?.LengthMm.HasValue == true && bolts.LengthMm!.Value > 0;
            double length = fromSpec ? bolts!.LengthMm!.Value : limits.ComputeBoltLengthMm(grip, diameter);
            return new BoltStack(gusset, plateThickness, side, length, addition, fromSpec);
        }
    }
}
