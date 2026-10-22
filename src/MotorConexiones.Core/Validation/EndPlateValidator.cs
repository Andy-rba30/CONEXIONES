using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Units;

namespace MotorConexiones.Core.Validation
{
    /// <summary>Huella del tubo en el plano de la plancha (ancho a lo largo de X, alto a lo largo de Y), en mm.</summary>
    public readonly struct TubeFootprint
    {
        public TubeFootprint(double widthMm, double heightMm)
        {
            WidthMm = widthMm;
            HeightMm = heightMm;
        }

        public double WidthMm { get; }
        public double HeightMm { get; }

        /// <summary>Distancia de un punto a la cara del tubo más cercana (negativa si el punto cae dentro de la huella).</summary>
        public double DistanceTo(double x, double y)
        {
            double dx = Math.Abs(x) - WidthMm / 2.0;
            double dy = Math.Abs(y) - HeightMm / 2.0;
            if (dx <= 0 && dy <= 0) return Math.Max(dx, dy);
            if (dx <= 0) return dy;
            if (dy <= 0) return dx;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

    /// <summary>
    /// Fase 12: las reglas de <c>end_plate</c> (empalme con dos planchas cara a cara): rótulos (espesor, diámetro, agujero),
    /// perfiles, los dos tramos colineales y del mismo perfil, la plancha cubre la huella del tubo, cada perno dentro de la
    /// plancha, a la distancia mínima del borde (del polígono con las esquinas recortadas) y de la cara del tubo, el paso
    /// mínimo, el agujero mayor que el perno, la longitud frente a la pila de dos planchas, la soldadura mínima y el retiro de
    /// cada tramo (espesor más media holgura). La huella del tubo: a lo ancho de la plancha (su X, perpendicular al plano de
    /// la cercha) va el lado del tubo perpendicular al plano (<c>HeightMm</c>, el 12" del Detalle C) y a lo alto el lado en el
    /// plano (<c>WidthMm</c>, el 8"); en un tubo cuadrado da igual.
    /// </summary>
    public static class EndPlateValidator
    {
        public const string TypeName = "end_plate";

        public static void Apply(ConnectionSpec spec, IModelFacts? facts, LimitsConfig limits, ValidationResult result)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            limits ??= LimitsConfig.Default;
            ValidationCommon.CheckUncertainFields(spec, result);
            if (spec.Chord == null || spec.Members == null || spec.Members.Count != 1 || spec.Plate == null || spec.Bolts == null)
            {
                result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "end_plate necesita chord (primer tramo), members[0] (segundo tramo), plate y bolts.", "", "Copia el ejemplo de conn_get_schema end_plate."));
                return;
            }
            ValidationCommon.CheckDimensionChains(spec, limits, result);
            EndPlateSpec plate = spec.Plate;
            RingBoltSpec bolts = spec.Bolts;
            MemberSpec second = spec.Members[0];
            // Rótulos.
            ValidationCommon.CheckLabel(plate.ThicknessLabel, plate.ThicknessMm, "espesor de la plancha", "plate.thickness_label", limits, result);
            ValidationCommon.CheckLabel(bolts.DiameterLabel, bolts.DiameterMm, "diámetro de los pernos", "bolts.diameter_label", limits, result);
            ValidationCommon.CheckLabel(bolts.HoleLabel, bolts.HoleMm, "diámetro del agujero", "bolts.hole_label", limits, result);
            // Perfiles.
            ValidationCommon.CheckProfile(spec.Chord.ElementId, spec.Chord.Profile, "chord.profile", "el primer tramo", facts, result);
            ValidationCommon.CheckProfile(second.ElementId, second.Profile, "members[0].profile", "el segundo tramo", facts, result);
            MemberModelFacts? a = facts?.GetMemberFacts(spec.Chord.ElementId);
            MemberModelFacts? b = facts?.GetMemberFacts(second.ElementId);
            if (a != null && b != null)
            {
                Vec3 da = (a.CurveEndMm - a.CurveStartMm).Normalized();
                Vec3 db = (b.CurveEndMm - b.CurveStartMm).Normalized();
                double sin = da.Cross(db).Length;
                if (sin > Math.Sin(5.0 * Math.PI / 180.0))
                {
                    double degrees = Math.Asin(Math.Min(1.0, sin)) * 180.0 / Math.PI;
                    result.Errors.Add(new ApiError(ErrorCodes.SpliceNotCollinear,
                        $"Los dos tramos ({spec.Chord.ElementId} y {second.ElementId}) no son colineales: forman {degrees:F1}° entre sí.",
                        "members[0].element_id", "Una placa de extremo empalma dos tramos de la misma barra; con un quiebro mayor de 5° no es un empalme."));
                }
                if (!string.Equals(a.TypeName, b.TypeName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Warnings.Add(new ApiError(ErrorCodes.SpliceProfilesDiffer,
                        $"Los dos tramos son de perfiles distintos ({a.TypeName} y {b.TypeName}): la plancha se dimensiona con el mayor.",
                        "members[0].profile", "Comprueba en el plano si el empalme cambia de perfil."));
                }
            }
            // Huella del tubo (del modelo o del perfil escrito).
            TubeFootprint tube = Footprint(a, b, spec);
            double t = plate.ThicknessMm ?? 0.0;
            double w = plate.WidthMm ?? 0.0;
            double h = plate.HeightMm ?? 0.0;
            double corner = Math.Max(0.0, plate.CornerCutMm ?? 0.0);
            double gap = Math.Max(0.0, spec.GapMm ?? 0.0);
            if (w > 0 && h > 0 && (tube.WidthMm > w - 1e-6 || tube.HeightMm > h - 1e-6))
            {
                result.Errors.Add(new ApiError(ErrorCodes.PlateSmallerThanTube,
                    $"La plancha ({w:F0} × {h:F0}) no cubre la huella del tubo ({tube.WidthMm:F1} × {tube.HeightMm:F1}).",
                    "plate.width_mm", "Agranda la plancha o revisa el perfil del tubo."));
            }
            // Pernos.
            double diameter = bolts.DiameterMm ?? 0.0;
            if (diameter > 0 && w > 0 && h > 0)
            {
                Polygon2D polygon = Octagon.Polygon(w, h, corner);
                double minEdge = limits.GetMinBoltEdgeDistance(diameter);
                double minSpacing = limits.Bolts.MinSpacingFactor * diameter;
                double minToTube = limits.EndPlate?.MinBoltToTubeMm ?? 20.0;
                foreach (BoltRow row in BoltRing.Rows(plate, bolts))
                {
                    string path = "bolts.faces." + row.Face;
                    if (row.Count > 1 && row.SpacingMm < minSpacing - 0.01)
                    {
                        result.Errors.Add(new ApiError(ErrorCodes.BoltSpacingTooSmall,
                            $"El paso de la fila {row.Face} ({row.SpacingMm:F1} mm) es menor que el mínimo AISC ({minSpacing:F1} mm = {limits.Bolts.MinSpacingFactor:F3} × d) para pernos de {diameter:F2} mm.",
                            path + ".spacing_mm", $"Aumenta spacing_mm a al menos {Math.Ceiling(minSpacing)} mm."));
                    }
                    foreach (BoltPosition p in row.Positions)
                    {
                        var point = new Point2D(p.X, p.Y);
                        if (!polygon.ContainsPoint(point))
                        {
                            result.Errors.Add(new ApiError(ErrorCodes.BoltOutsidePlate,
                                $"Un perno de la fila {row.Face} ({p.X:F1}, {p.Y:F1}) queda fuera de la plancha.",
                                path, "Reduce el paso o el borde, o agranda la plancha."));
                            continue;
                        }
                        double edge = Geometry2DChecks.DistanceToBoundary(polygon, point);
                        if (edge < minEdge - 0.01)
                        {
                            result.Errors.Add(new ApiError(ErrorCodes.BoltEdgeDistanceTooSmall,
                                $"Un perno de la fila {row.Face} queda a {edge:F1} mm del borde de la plancha (mínimo {minEdge:F1} mm para pernos de {bolts.DiameterLabel ?? (diameter.ToString("F1", CultureInfo.InvariantCulture) + " mm")}).",
                                path + ".edge_mm", $"Revisa la cota en el plano o usa edge_mm >= {minEdge:F0} (y vigila las esquinas recortadas)."));
                        }
                        double toTube = tube.DistanceTo(p.X, p.Y);
                        if (toTube < minToTube - 0.01)
                        {
                            result.Errors.Add(new ApiError(ErrorCodes.BoltTooCloseToTube,
                                $"Un perno de la fila {row.Face} queda a {toTube:F1} mm de la cara del tubo (mínimo {minToTube:F1} mm para la tuerca y la arandela).",
                                path + ".edge_mm", "Acerca la fila al borde de la plancha (menos edge_mm) o agranda la plancha."));
                        }
                    }
                }
                if (BoltRing.Count(bolts) == 0)
                {
                    result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "La placa de extremo no tiene ningún perno (todas las filas con count 0).", "bolts.faces", "Pon al menos un perno por cara, como los Detalles B, C e I."));
                }
                double clearance = limits.EndPlate?.MinHoleClearanceMm ?? 1.0;
                if (bolts.HoleMm.HasValue && bolts.HoleMm.Value < diameter + clearance - 0.01)
                {
                    result.Errors.Add(new ApiError(ErrorCodes.BoltHoleTooSmall,
                        $"El agujero ({bolts.HoleMm.Value:F2} mm) debe ser al menos {clearance:F1} mm mayor que el perno ({diameter:F2} mm).",
                        "bolts.hole_mm", "Ø18 para 5/8\" y Ø13/16\" para 3/4\" en los detalles del plano."));
                }
                if (t > 0)
                {
                    BoltStack stack = BoltStack.ForTwoPlates(t, t, gap, bolts, limits);
                    if (bolts.LengthMm.HasValue && bolts.LengthMm.Value < stack.MinimumLengthMm - 0.01)
                    {
                        result.Warnings.Add(new ApiError(ErrorCodes.BoltLengthTooShort,
                            $"La longitud del perno ({bolts.LengthMm.Value:F1} mm) es menor que el agarre de las dos planchas ({stack.GripMm:F1} mm) más el suplemento de tuerca, arandela y rosca ({stack.LengthAdditionMm:F1} mm).",
                            "bolts.length_mm", $"Usa length_mm >= {Math.Ceiling(stack.MinimumLengthMm)} mm o quítalo para que se calcule ({limits.ComputeBoltLengthMm(stack.GripMm, diameter):F2} mm)."));
                    }
                }
            }
            // Soldadura de la plancha al tubo.
            double wall = a != null && a.ThicknessMm > 0 ? a.ThicknessMm : (b != null && b.ThicknessMm > 0 ? b.ThicknessMm : 6.35);
            ValidationCommon.CheckWeld(plate.Weld, Math.Min(t > 0 ? t : wall, wall), "plate.weld.size_mm", "la plancha al tubo", limits, result);
            // Retiros (D8): cada tramo deja sitio a su plancha: espesor más media holgura, menos el hueco que el tramo ya deja en el
            // modelo entre su extremo y el punto de empalme (0 si los dos tramos se tocan).
            double needed = t + gap / 2.0;
            if (t > 0)
            {
                double gapA = 0.0, gapB = 0.0;
                if (a != null && b != null)
                {
                    Vec3 splicePoint = SpecFrame.SplicePoint(a.CurveStartMm, a.CurveEndMm, b.CurveStartMm, b.CurveEndMm);
                    gapA = NodeReach.EndGapMm(a.CurveStartMm, a.CurveEndMm, splicePoint);
                    gapB = NodeReach.EndGapMm(b.CurveStartMm, b.CurveEndMm, splicePoint);
                }
                double neededA = Math.Max(0.0, needed - gapA);
                double neededB = Math.Max(0.0, needed - gapB);
                if (spec.Chord.EndSetbackMm.HasValue && spec.Chord.EndSetbackMm.Value < neededA - 0.05)
                {
                    result.Errors.Add(new ApiError(ErrorCodes.SpliceSetbackTooSmall,
                        $"El retiro del primer tramo ({spec.Chord.EndSetbackMm.Value:F1} mm) no deja sitio a su plancha ({neededA:F1} mm = espesor más media holgura{(gapA > 0.05 ? $" menos el hueco de {gapA:F1} mm que ya deja el tramo" : "")}).",
                        "chord.end_setback_mm", $"Usa end_setback_mm >= {neededA:F1}."));
                }
                if (second.EndSetbackMm.HasValue && second.EndSetbackMm.Value < neededB - 0.05)
                {
                    result.Errors.Add(new ApiError(ErrorCodes.SpliceSetbackTooSmall,
                        $"El retiro del segundo tramo ({second.EndSetbackMm.Value:F1} mm) no deja sitio a su plancha ({neededB:F1} mm = espesor más media holgura{(gapB > 0.05 ? $" menos el hueco de {gapB:F1} mm que ya deja el tramo" : "")}).",
                        "members[0].end_setback_mm", $"Usa end_setback_mm >= {neededB:F1}."));
                }
            }
            ValidationCommon.CheckElementsReachNode(spec, facts, limits, result);
        }

        /// <summary>
        /// La huella del tubo en la plancha: a lo ancho de la plancha (su X, perpendicular al plano de la cercha) el lado del tubo
        /// perpendicular al plano (<see cref="MemberModelFacts.HeightMm"/>) y a lo alto el lado en el plano
        /// (<see cref="MemberModelFacts.WidthMm"/>). Ronda 12c: antes se adivinaba con el mayor y el menor porque el add-in leía
        /// el HSS12X8 como 203 × 203; ahora los hechos ya vienen orientados (<see cref="SectionOrientation"/>). Sin hechos del
        /// modelo, del perfil escrito con el canto a lo ancho.
        /// </summary>
        public static TubeFootprint Footprint(MemberModelFacts? a, MemberModelFacts? b, ConnectionSpec spec)
        {
            double width = 0, height = 0;
            foreach (MemberModelFacts? f in new[] { a, b })
            {
                if (f == null) continue;
                double inPlane = f.WidthMm > 0 ? f.WidthMm : f.HeightMm;
                double outOfPlane = f.HeightMm > 0 ? f.HeightMm : f.WidthMm;
                width = Math.Max(width, outOfPlane);
                height = Math.Max(height, inPlane);
            }
            if (width <= 0)
            {
                string? profile = spec.Chord?.Profile ?? spec.Members?.FirstOrDefault()?.Profile;
                if (!string.IsNullOrWhiteSpace(profile) && ProfileMatcher.TryParseHssDimensions(profile!, out double wi, out double hi, out _))
                {
                    width = UnitConverter.InchesToMm(Math.Max(wi, hi));
                    height = UnitConverter.InchesToMm(Math.Min(wi, hi) > 0 ? Math.Min(wi, hi) : Math.Max(wi, hi));
                }
            }
            return new TubeFootprint(width, height > 0 ? height : width);
        }
    }
}
