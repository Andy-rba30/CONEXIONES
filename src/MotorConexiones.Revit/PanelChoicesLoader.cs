using System;
using System.IO;
using System.Reflection;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Fase 11: lee <c>config\choices.json</c> de la carpeta del add-in desplegado (como <see cref="LimitsConfigLoader"/>) y
    /// deja en la lista de diámetros de perno solo los que existen en <c>limits.json</c> (los descartados van al log). Sin
    /// archivo, las listas embebidas del Core: nunca un panel vacío.
    /// </summary>
    internal static class PanelChoicesLoader
    {
        public static string ConfigPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "config", "choices.json");

        public static PanelChoices Load(LimitsConfig? limits = null)
        {
            PanelChoices choices;
            try
            {
                choices = PanelChoices.LoadFromFile(ConfigPath);
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "choices_load_failed", file = ConfigPath, error = ex.Message });
                choices = PanelChoices.Default;
            }
            try
            {
                int discarded = choices.FilterBoltDiameters(limits ?? LimitsConfigLoader.Load());
                if (discarded > 0)
                {
                    JsonLineLogger.Write(new { @event = "choices_bolt_labels_discarded", file = ConfigPath, discarded = choices.DiscardedBoltLabels, kept = choices.BoltDiameterLabels });
                }
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "choices_load_failed", file = ConfigPath, error = ex.Message });
            }
            return choices;
        }
    }
}
