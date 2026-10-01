using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Services
{
    /// <summary>Resultado de validar una especificación contra el modelo abierto (lo mismo que <c>conn_validate</c>).</summary>
    public sealed class ModelValidation
    {
        public ModelValidation(ConnectionSpec? spec, NodeFrame? frame, ApiError? nodeError, ValidationResult result, RevitModelFacts facts, LimitsConfig limits)
        {
            Spec = spec;
            Frame = frame;
            NodeError = nodeError;
            Result = result;
            Facts = facts;
            Limits = limits;
        }

        public ConnectionSpec? Spec { get; }
        public NodeFrame? Frame { get; }
        public ApiError? NodeError { get; }
        public ValidationResult Result { get; }
        public RevitModelFacts Facts { get; }
        public LimitsConfig Limits { get; }
        public bool IsValid => Result.IsValid && !string.IsNullOrEmpty(Result.ValidationToken);
    }

    /// <summary>
    /// La validación contra el modelo en un solo sitio (Fase 7): marco del nudo con <see cref="NodeInspector.ResolveNode"/>,
    /// hechos con <see cref="RevitModelFacts"/>, <see cref="SpecValidator"/> con <c>config\limits.json</c>, y los fallos
    /// del nudo que el validador no cubre añadidos como errores. La usan <c>conn_catalog_apply</c>, <c>conn_catalog_save</c>
    /// y la ventana de previsualización; <c>conn_validate</c> hace lo mismo paso a paso.
    /// </summary>
    internal static class ValidationService
    {
        public static ModelValidation Validate(Document document, string rawJson, ConnectionSpec? spec = null)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            LimitsConfig limits = LimitsConfigLoader.Load();
            ApiError? parseError = null;
            if (spec == null)
            {
                try
                {
                    spec = ConnectionSpec.FromJson(rawJson);
                }
                catch (Exception ex)
                {
                    parseError = new ApiError(ErrorCodes.SchemaInvalid, "No se pudo deserializar la especificación: " + ex.Message, "", "Corrige la sintaxis JSON.");
                }
            }

            NodeFrame? frame = null;
            ApiError? nodeError = null;
            if (spec != null)
            {
                try
                {
                    frame = NodeInspector.ResolveNode(document, spec).Frame;
                }
                catch (NodeInspectionException ex)
                {
                    nodeError = ex.Error;
                }
                catch (Exception ex)
                {
                    nodeError = new ApiError(ErrorCodes.InternalError, "No se pudo leer el nudo del modelo: " + ex.Message, "node", "Revisa los element_ids.");
                }
            }

            var facts = new RevitModelFacts(document, frame);
            ValidationResult result = SpecValidator.Validate(rawJson, spec, facts, limits);
            if (parseError != null && !result.Errors.Any(e => e.Code == parseError.Code))
            {
                result.Errors.Add(parseError);
                result.ValidationToken = null;
            }
            if (nodeError != null && !result.Errors.Any(e => e.Code == nodeError.Code))
            {
                result.Errors.Add(nodeError);
                result.ValidationToken = null;
            }
            return new ModelValidation(spec, frame, nodeError, result, facts, limits);
        }

        /// <summary><c>calculated_values</c> de <c>conn_validate</c>: origen y ejes del marco canónico.</summary>
        public static object? CalculatedValues(NodeFrame? frame)
        {
            if (frame == null) return null;
            return new
            {
                origin_mm = new[] { Math.Round(frame.Origin.X, 1), Math.Round(frame.Origin.Y, 1), Math.Round(frame.Origin.Z, 1) },
                axis_distance_mm = Math.Round(frame.AxisDistanceMm, 2),
                frame_x = new[] { Math.Round(frame.X.X, 4), Math.Round(frame.X.Y, 4), Math.Round(frame.X.Z, 4) },
                frame_y = new[] { Math.Round(frame.Y.X, 4), Math.Round(frame.Y.Y, 4), Math.Round(frame.Y.Z, 4) },
                frame_z = new[] { Math.Round(frame.Z.X, 4), Math.Round(frame.Z.Y, 4), Math.Round(frame.Z.Z, 4) },
                chord_direction_reversed = frame.ChordReversed,
            };
        }

        /// <summary><c>bolt_stacks</c> de <c>conn_validate</c> (ronda 6b): agarre y longitud de cada grupo de pernos.</summary>
        public static List<object> BoltStacks(ConnectionSpec? spec, LimitsConfig limits)
        {
            var stacks = new List<object>();
            if (spec?.Members == null) return stacks;
            double gussetThickness = spec.Gusset?.ThicknessMm ?? 9.525;
            foreach (var member in spec.Members)
            {
                var plate = member.Attachment?.Plate;
                if (plate == null) continue;
                var stack = BoltStack.Compute(gussetThickness, plate, member.Attachment?.Bolts, limits);
                stacks.Add(new
                {
                    member_element_id = member.ElementId,
                    gusset_face = stack.FaceLabel,
                    grip_mm = Math.Round(stack.GripMm, 3),
                    bolt_length_mm = Math.Round(stack.BoltLengthMm, 3),
                    length_source = stack.LengthFromSpec ? "spec" : "computed_from_grip",
                });
            }
            return stacks;
        }
    }
}
