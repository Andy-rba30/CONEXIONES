using System;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Paquete que atraviesan los pernos de una placa cuchilla: la cartela (centrada en el plano del nudo, z = 0) y la
    /// placa cuchilla apoyada sobre la cara +Z de la cartela (solape). Todo en mm, en el eje Z del sistema local.
    /// Es el dato que faltaba en la Fase 5: la placa cuchilla y la cartela se creaban en el mismo plano (se cruzaban)
    /// y los pernos, de 45 mm fijos, salían del plano medio de la cartela sin atravesar nada
    /// (docs/fases/resultados-fase-6.md, capturas fase6-05). Ahora:
    /// - la placa cuchilla se desplaza a <see cref="KnifePlateZOffsetMm"/> = (t_cartela + t_cuchilla) / 2;
    /// - los pernos entran por la cara exterior de la placa cuchilla (<see cref="OuterFaceZMm"/>) con la cabeza
    ///   en esa cara y el vástago hacia −Z, y salen por la cara −Z de la cartela (<see cref="InnerFaceZMm"/>);
    /// - la longitud del perno es agarre + suplemento de la tabla 7-15 del AISC Manual (config/limits.json),
    ///   redondeada hacia arriba a 1/4": para 5/8" con 9,525 + 10 mm de agarre, 1-3/4" = 44,45 mm.
    /// </summary>
    public sealed class BoltStack
    {
        public const string GussetFace = "+z";

        public BoltStack(double gussetThicknessMm, double knifeThicknessMm, double diameterMm, double lengthAdditionMm, double lengthIncrementMm)
        {
            if (gussetThicknessMm <= 0) throw new ArgumentException("El espesor de la cartela debe ser positivo.", nameof(gussetThicknessMm));
            if (knifeThicknessMm <= 0) throw new ArgumentException("El espesor de la placa cuchilla debe ser positivo.", nameof(knifeThicknessMm));
            if (diameterMm <= 0) throw new ArgumentException("El diámetro del perno debe ser positivo.", nameof(diameterMm));
            if (lengthIncrementMm <= 0) throw new ArgumentException("El paso de longitudes de perno debe ser positivo.", nameof(lengthIncrementMm));

            GussetThicknessMm = gussetThicknessMm;
            KnifeThicknessMm = knifeThicknessMm;
            DiameterMm = diameterMm;
            LengthAdditionMm = Math.Max(0.0, lengthAdditionMm);
            LengthIncrementMm = lengthIncrementMm;
            GripMm = gussetThicknessMm + knifeThicknessMm;
            KnifePlateZOffsetMm = (gussetThicknessMm + knifeThicknessMm) / 2.0;
            OuterFaceZMm = gussetThicknessMm / 2.0 + knifeThicknessMm;
            InnerFaceZMm = -gussetThicknessMm / 2.0;
            BoltLengthMm = RoundUp(GripMm + LengthAdditionMm, lengthIncrementMm);
        }

        /// <summary>Paquete a partir de los espesores del contrato y la tabla de suplementos de <paramref name="limits"/>.</summary>
        public static BoltStack Compute(double gussetThicknessMm, double knifeThicknessMm, double diameterMm, LimitsConfig? limits)
        {
            LimitsConfig config = limits ?? LimitsConfig.Default;
            return new BoltStack(gussetThicknessMm, knifeThicknessMm, diameterMm, config.GetBoltLengthAddition(diameterMm), config.Bolts.LengthIncrementMm);
        }

        public double GussetThicknessMm { get; }
        public double KnifeThicknessMm { get; }
        public double DiameterMm { get; }

        /// <summary>Suplemento sumado al agarre (AISC Manual, tabla 7-15).</summary>
        public double LengthAdditionMm { get; }

        /// <summary>Paso comercial de longitudes (6,35 mm = 1/4").</summary>
        public double LengthIncrementMm { get; }

        /// <summary>Agarre: cartela + placa cuchilla.</summary>
        public double GripMm { get; }

        /// <summary>Plano medio de la placa cuchilla respecto al plano del nudo (positivo: cara +Z de la cartela).</summary>
        public double KnifePlateZOffsetMm { get; }

        /// <summary>Cara exterior de la placa cuchilla: donde apoya la cabeza del perno.</summary>
        public double OuterFaceZMm { get; }

        /// <summary>Cara de la cartela opuesta a la placa cuchilla: por donde sale el vástago con la tuerca.</summary>
        public double InnerFaceZMm { get; }

        /// <summary>Longitud comercial del perno.</summary>
        public double BoltLengthMm { get; }

        /// <summary>Lo que sobresale del paquete por la cara de la tuerca.</summary>
        public double ProtrusionMm => BoltLengthMm - GripMm;

        /// <summary>Redondeo hacia arriba al paso indicado (con tolerancia numérica para no saltar un paso por 1e-9).</summary>
        public static double RoundUp(double valueMm, double stepMm)
        {
            if (stepMm <= 0) return valueMm;
            double steps = Math.Ceiling(valueMm / stepMm - 1e-9);
            return steps * stepMm;
        }
    }
}
