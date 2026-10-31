using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Fase 13 (decisiones D6, D7 y D8; sección 3.3 del prompt): parte una barra de armazón en dos elementos de Revit en el punto
    /// que dice <see cref="SplitPoint"/>, y deshace el corte. Mecanismo (a): <c>ElementTransformUtils.CopyElement</c> y después la
    /// <c>LocationCurve.Curve</c> de cada uno (conserva tipo, nivel, giro de sección, justificación y desfases); reserva (b): si la
    /// copia no sale, <c>Document.Create.NewFamilyInstance(curva, símbolo, nivel, Beam)</c> y los parámetros de instancia copiados a
    /// mano. En los dos casos: el <b>tramo corto</b> (del lado del cordón) conserva el elemento original (D6); las extensiones del
    /// extremo que no se tocó se conservan y las del corte valen 0; las uniones automáticas en el corte se desactivan
    /// (<c>StructuralFramingUtils.DisallowJoinAtEnd</c>); los parámetros que no coinciden tras copiar se escriben a mano y, si
    /// tampoco, se anotan (<c>SPLIT_PARAMETERS_APPROXIMATED</c>). <b>Probado en Revit el 2026-10-31</b> (instalación de la Fase 13,
    /// <c>resultados-fase-13.md</c>): (a) partió las dos diagonales del nudo en K del Hangar con <c>copy_element</c>, sin parámetros
    /// aproximados, y el deshacer dejó cada barra entera; la reserva (b) no hizo falta. Desde la 0.13.1 (cierre de la fase) los dos
    /// tramos se construyen sobre la <b>línea exacta de Revit</b> (el punto de corte proyectado en pies), no desde los mm del Core.
    /// Llamar siempre dentro de una Transaction; el llamador guarda el registro.
    /// </summary>
    public static class MemberSplitter
    {
        public const string MechanismCopy = "copy_element";
        public const string MechanismNew = "new_family_instance";

        /// <summary>Parámetros de instancia que el tramo nuevo debe conservar del original.</summary>
        private static readonly BuiltInParameter[] InstanceParameters =
        {
            BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE,
            BuiltInParameter.Y_JUSTIFICATION,
            BuiltInParameter.Z_JUSTIFICATION,
            BuiltInParameter.Y_OFFSET_VALUE,
            BuiltInParameter.Z_OFFSET_VALUE,
            BuiltInParameter.STRUCTURAL_MATERIAL_PARAM,
        };

        /// <summary>
        /// Parte <paramref name="member"/> en <paramref name="result"/>. Devuelve el registro (sin guardar) con el tramo nuevo. Lanza
        /// <see cref="CatalogException"/> <c>SPLIT_FAILED</c> si Revit no pudo partir; en ese caso nada cambió (la Transaction del
        /// llamador se revierte).
        /// </summary>
        public static SplitRecord Split(Document document, FamilyInstance member, SplitPointResult result, long chordElementId, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (result == null) throw new ArgumentNullException(nameof(result));
            warnings ??= new List<ApiError>();
            if (member.Location is not LocationCurve location || location.Curve == null)
            {
                throw new CatalogException(ErrorCodes.SplitFailed, "La barra " + member.Id.Value + " no tiene línea de ubicación: no se puede partir.", "element_id",
                    "Solo se parten barras de armazón estructural con eje (LocationCurve).");
            }
            if (location.Curve is not Line originalLine)
            {
                throw new CatalogException(ErrorCodes.SplitFailed, "La barra " + member.Id.Value + " no es recta (" + location.Curve.GetType().Name + "): el corte del Detalle B solo vale para barras rectas.", "element_id",
                    "Solo se parten barras de armazón estructural con eje recto (Line).");
            }
            XYZ exactStart = originalLine.GetEndPoint(0);
            XYZ exactEnd = originalLine.GetEndPoint(1);
            Vec3 p0 = RevitGeometry.ToMm(exactStart);
            Vec3 p1 = RevitGeometry.ToMm(exactEnd);
            var facts = new Core.Model.MemberModelFacts { ElementId = member.Id.Value, CurveStartMm = p0, CurveEndMm = p1, TypeName = member.Symbol.Name };
            var (shortStart, shortEnd, longStart, longEnd) = SplitPoint.Segments(facts, result); // valida que el corte cae dentro de la barra y da las longitudes
            int near = SplitPoint.NearEndIndex(facts, result.CutPointMm);

            // Cierre de la Fase 13 (0.13.1): el punto de corte se proyecta sobre la línea real de Revit y los dos tramos se construyen
            // con los extremos exactos del original (en pies, sin pasar por mm), de modo que queden exactamente sobre su eje. En la
            // instalación de la Fase 13 el tramo nuevo salió con la advertencia de Revit "Beam or Brace is slightly off axis"; si con
            // esto no desaparece, la desviación es de la barra original (el sondeo 30 la mide).
            XYZ splitFeet = originalLine.Project(RevitGeometry.ToFeet(near == 0 ? shortEnd : shortStart)).XYZPoint;
            XYZ nearFeet = near == 0 ? exactStart : exactEnd;
            XYZ farFeet = near == 0 ? exactEnd : exactStart;
            Line shortLine = near == 0 ? Line.CreateBound(nearFeet, splitFeet) : Line.CreateBound(splitFeet, nearFeet);
            Line longLine = near == 0 ? Line.CreateBound(splitFeet, farFeet) : Line.CreateBound(farFeet, splitFeet);

            var record = new SplitRecord
            {
                ElementId = member.Id.Value,
                ChordElementId = chordElementId,
                OriginalStartMm = new[] { p0.X, p0.Y, p0.Z },
                OriginalEndMm = new[] { p1.X, p1.Y, p1.Z },
                OriginalStartFeet = new[] { exactStart.X, exactStart.Y, exactStart.Z },
                OriginalEndFeet = new[] { exactEnd.X, exactEnd.Y, exactEnd.Z },
                SplitPointMm = new[] { result.SplitPointMm.X, result.SplitPointMm.Y, result.SplitPointMm.Z },
                OffsetFromChordFaceMm = Math.Round(result.FirstPlateDistanceMm - result.FaceDistanceMm, 3),
                SplitDistanceMm = Math.Round(result.SplitDistanceMm, 3),
            };
            foreach (BuiltInParameter extension in new[] { BuiltInParameter.START_EXTENSION, BuiltInParameter.END_EXTENSION })
            {
                Parameter? parameter = member.get_Parameter(extension);
                if (parameter != null && parameter.HasValue) record.OriginalExtensionsFeet[extension.ToString()] = parameter.AsDouble();
            }
            // 0.13.2: el estado original de las uniones automáticas en cada extremo, para que Unsplit lo deje igual (no "permitidas").
            try
            {
                record.OriginalJoinAllowed = new[] { StructuralFramingUtils.IsJoinAllowedAtEnd(member, 0), StructuralFramingUtils.IsJoinAllowedAtEnd(member, 1) };
            }
            catch
            {
                record.OriginalJoinAllowed = null;
            }
            var originalValues = InstanceParameters.ToDictionary(p => p, p => ReadValue(member, p));

            // (a) Copiar el elemento en su sitio y asignar a cada uno su tramo.
            FamilyInstance? copy = null;
            string mechanism = MechanismCopy;
            try
            {
                ICollection<ElementId> copied = ElementTransformUtils.CopyElement(document, member.Id, XYZ.Zero);
                copy = copied.Select(id => document.GetElement(id)).OfType<FamilyInstance>().FirstOrDefault();
                if (copy == null)
                {
                    foreach (ElementId id in copied) document.Delete(id);
                }
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "split_copy_failed", element_id = member.Id.Value, error = error.Message });
                copy = null;
            }

            // (b) Reserva: crear el tramo nuevo con el mismo símbolo y nivel y copiar los parámetros a mano.
            if (copy == null)
            {
                mechanism = MechanismNew;
                try
                {
                    Level? level = document.GetElement(member.LevelId) as Level;
                    copy = level != null
                        ? document.Create.NewFamilyInstance(longLine, member.Symbol, level, StructuralType.Beam)
                        : document.Create.NewFamilyInstance(longLine, member.Symbol, (Level?)null, StructuralType.Beam);
                }
                catch (Exception error)
                {
                    throw new CatalogException(ErrorCodes.SplitFailed,
                        "Revit no pudo partir la barra " + member.Id.Value + ": ni la copia del elemento ni NewFamilyInstance crearon el tramo nuevo (" + error.Message + ").",
                        "element_id", "Nada cambió en el modelo. Mira el log (split_copy_failed) y el sondeo 30 de docs/instalacion/fase-13.md.");
                }
            }
            if (copy == null)
            {
                throw new CatalogException(ErrorCodes.SplitFailed, "Revit no devolvió el tramo nuevo al partir la barra " + member.Id.Value + ".", "element_id", "Nada cambió en el modelo.");
            }

            // Las curvas: el original se queda con el tramo corto (D6), el nuevo con el largo.
            try
            {
                location.Curve = shortLine;
                if (copy.Location is LocationCurve copyLocation)
                {
                    copyLocation.Curve = longLine;
                }
                else
                {
                    throw new InvalidOperationException("el tramo nuevo no tiene LocationCurve");
                }
            }
            catch (Exception error)
            {
                throw new CatalogException(ErrorCodes.SplitFailed, "Revit no dejó asignar las curvas de los dos tramos de la barra " + member.Id.Value + ": " + error.Message, "element_id",
                    "Nada cambió en el modelo (la operación se revierte entera).");
            }

            // Extensiones: el extremo del corte a 0 en los dos tramos; el extremo que no se tocó conserva la suya.
            int shortSplitEnd = near == 0 ? 1 : 0;
            int longSplitEnd = near == 0 ? 0 : 1;
            SetExtension(member, shortSplitEnd, 0.0);
            SetExtension(copy, longSplitEnd, 0.0);
            SetExtension(copy, 1 - longSplitEnd, record.OriginalExtensionsFeet.TryGetValue((near == 0 ? BuiltInParameter.END_EXTENSION : BuiltInParameter.START_EXTENSION).ToString(), out double farExtension) ? farExtension : 0.0);

            // Uniones automáticas en el corte: desactivadas (si la API lo permite para este elemento).
            TryDisallowJoin(member, shortSplitEnd, warnings);
            TryDisallowJoin(copy, longSplitEnd, warnings);

            // Parámetros de instancia del tramo nuevo: iguales que el original; si no, se escriben; si tampoco, se anotan.
            var approximated = new List<string>();
            foreach (BuiltInParameter parameter in InstanceParameters)
            {
                string original = originalValues[parameter];
                string current = ReadValue(copy, parameter);
                if (string.Equals(original, current, StringComparison.Ordinal)) continue;
                if (!TryCopyValue(member, copy, parameter) || !string.Equals(ReadValue(copy, parameter), original, StringComparison.Ordinal))
                {
                    approximated.Add(parameter + " (original " + original + ", tramo nuevo " + ReadValue(copy, parameter) + ")");
                }
            }
            if (approximated.Count > 0)
            {
                warnings.Add(new ApiError(ErrorCodes.SplitParametersApproximated,
                    "El tramo nuevo " + copy.Id.Value + " no conserva " + approximated.Count + " parámetro(s) del original " + member.Id.Value + ": " + string.Join("; ", approximated) + ".",
                    "element_id", "Revísalos en Propiedades (giro de sección, justificación, desfases). El sondeo 30 dice si el mecanismo de copia los conserva."));
            }

            record.NewElementId = copy.Id.Value;
            record.Mechanism = mechanism;
            record.ApproximatedParameters = approximated;
            JsonLineLogger.Write(new
            {
                @event = "member_split",
                element_id = member.Id.Value,
                new_element_id = copy.Id.Value,
                mechanism,
                split_point_mm = record.SplitPointMm,
                split_distance_mm = record.SplitDistanceMm,
                short_mm = Math.Round(shortStart.DistanceTo(shortEnd), 1),
                long_mm = Math.Round(longStart.DistanceTo(longEnd), 1),
                approximated,
            });
            return record;
        }

        /// <summary>
        /// Deshace un corte: borra el tramo nuevo y devuelve al original su curva y sus extensiones. Devuelve falso (con el aviso
        /// <c>SPLIT_UNDO_PARTIAL</c>) si algo no volvió a su sitio; la barra original nunca se borra. Llamar dentro de una Transaction.
        /// </summary>
        public static bool Unsplit(Document document, SplitRecord record, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (record == null) throw new ArgumentNullException(nameof(record));
            warnings ??= new List<ApiError>();
            var problems = new List<string>();
            Element? original = document.GetElement(new ElementId(record.ElementId));
            Element? created = document.GetElement(new ElementId(record.NewElementId));
            if (created != null)
            {
                try
                {
                    document.Delete(created.Id);
                }
                catch (Exception error)
                {
                    problems.Add("no se pudo borrar el tramo nuevo " + record.NewElementId + " (" + error.Message + ")");
                }
            }
            if (original is FamilyInstance member && member.Location is LocationCurve location)
            {
                try
                {
                    // 0.13.1: la curva exacta en pies si el registro la trae; los registros de la 0.13.0 se restauran desde los mm.
                    XYZ start = record.OriginalStartFeet is { Length: 3 } startFeet
                        ? new XYZ(startFeet[0], startFeet[1], startFeet[2])
                        : RevitGeometry.ToFeet(new Vec3(record.OriginalStartMm[0], record.OriginalStartMm[1], record.OriginalStartMm[2]));
                    XYZ end = record.OriginalEndFeet is { Length: 3 } endFeet
                        ? new XYZ(endFeet[0], endFeet[1], endFeet[2])
                        : RevitGeometry.ToFeet(new Vec3(record.OriginalEndMm[0], record.OriginalEndMm[1], record.OriginalEndMm[2]));
                    location.Curve = Line.CreateBound(start, end);
                }
                catch (Exception error)
                {
                    problems.Add("la curva de la barra " + record.ElementId + " no se pudo restaurar (" + error.Message + ")");
                }
                foreach (var pair in record.OriginalExtensionsFeet)
                {
                    if (!Enum.TryParse(pair.Key, out BuiltInParameter bip)) continue;
                    Parameter? parameter = member.get_Parameter(bip);
                    if (parameter == null || parameter.IsReadOnly) continue;
                    try
                    {
                        parameter.Set(pair.Value);
                    }
                    catch (Exception error)
                    {
                        problems.Add(pair.Key + " de la barra " + record.ElementId + " no se pudo restaurar (" + error.Message + ")");
                    }
                }
                // Las uniones automáticas vuelven a su estado original (0.13.2: el que guardó el registro; en los registros de la
                // 0.13.0 y la 0.13.1, que no lo guardaban, se permiten en los dos extremos como hasta ahora).
                for (int end = 0; end < 2; end++)
                {
                    try
                    {
                        bool wanted = record.OriginalJoinAllowed is { Length: 2 } joins ? joins[end] : true;
                        bool current = StructuralFramingUtils.IsJoinAllowedAtEnd(member, end);
                        if (wanted && !current) StructuralFramingUtils.AllowJoinAtEnd(member, end);
                        else if (!wanted && current) StructuralFramingUtils.DisallowJoinAtEnd(member, end);
                    }
                    catch
                    {
                        // Si la API no lo permite para este elemento, la barra queda igual que tras el corte.
                    }
                }
            }
            else
            {
                problems.Add("la barra original " + record.ElementId + " no está en el modelo (o no es armazón): nada que restaurar");
            }
            JsonLineLogger.Write(new { @event = "member_unsplit", element_id = record.ElementId, new_element_id = record.NewElementId, problems });
            if (problems.Count > 0)
            {
                warnings.Add(new ApiError(ErrorCodes.SplitUndoPartial,
                    "El corte de la barra " + record.ElementId + " no se deshizo del todo: " + string.Join("; ", problems) + ".",
                    "element_id", "La barra original nunca se borra. Revisa la barra en Revit (Ctrl+Z deshace la operación entera si prefieres)."));
                return false;
            }
            return true;
        }

        private static void SetExtension(FamilyInstance member, int endIndex, double valueFeet)
        {
            Parameter? parameter = member.get_Parameter(endIndex == 0 ? BuiltInParameter.START_EXTENSION : BuiltInParameter.END_EXTENSION);
            if (parameter == null || parameter.IsReadOnly) return;
            try
            {
                parameter.Set(valueFeet);
            }
            catch
            {
                // Una extensión que no se deja escribir no impide partir; queda como la copió Revit.
            }
        }

        private static void TryDisallowJoin(FamilyInstance member, int endIndex, List<ApiError> warnings)
        {
            try
            {
                if (StructuralFramingUtils.IsJoinAllowedAtEnd(member, endIndex)) StructuralFramingUtils.DisallowJoinAtEnd(member, endIndex);
            }
            catch (Exception error)
            {
                warnings.Add(new ApiError(ErrorCodes.SplitParametersApproximated,
                    "No se pudo desactivar la unión automática en el corte de la barra " + member.Id.Value + " (extremo " + endIndex + "): " + error.Message,
                    "element_id", "Revit puede unir los dos tramos en el corte; revísalo en Revit (Editar unión de barras)."));
            }
        }

        private static string ReadValue(FamilyInstance member, BuiltInParameter parameter)
        {
            Parameter? p = member.get_Parameter(parameter);
            if (p == null || !p.HasValue) return "";
            switch (p.StorageType)
            {
                case StorageType.Double: return p.AsDouble().ToString("0.######", CultureInfo.InvariantCulture);
                case StorageType.Integer: return p.AsInteger().ToString(CultureInfo.InvariantCulture);
                case StorageType.ElementId: return p.AsElementId().Value.ToString(CultureInfo.InvariantCulture);
                default: return p.AsString() ?? "";
            }
        }

        private static bool TryCopyValue(FamilyInstance from, FamilyInstance to, BuiltInParameter parameter)
        {
            Parameter? source = from.get_Parameter(parameter);
            Parameter? target = to.get_Parameter(parameter);
            if (source == null || target == null || !source.HasValue || target.IsReadOnly) return false;
            try
            {
                switch (source.StorageType)
                {
                    case StorageType.Double: return target.Set(source.AsDouble());
                    case StorageType.Integer: return target.Set(source.AsInteger());
                    case StorageType.ElementId: return target.Set(source.AsElementId());
                    default: return target.Set(source.AsString() ?? "");
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Los dos tramos de un registro como hechos del modelo (para releer el empalme tras partir), o nulo si falta alguno.</summary>
        public static (FamilyInstance Short, FamilyInstance Long)? Segments(Document document, SplitRecord record)
        {
            FamilyInstance? a = document.GetElement(new ElementId(record.ElementId)) as FamilyInstance;
            FamilyInstance? b = document.GetElement(new ElementId(record.NewElementId)) as FamilyInstance;
            return a == null || b == null ? null : (a, b);
        }
    }
}
