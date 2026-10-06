using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Un botón de la columna "Qué hacer" (Fase 9, mejora C3): la clave la entiende la ventana; la etiqueta la lee la persona.</summary>
    public sealed class PlanAction
    {
        public const string Exclude = "exclude";
        public const string Include = "include";
        /// <summary>Volver a planificar un nudo que ya tiene conexión para rehacerla (<c>replace_existing</c>).</summary>
        public const string IncludeReplace = "include_replace";
        public const string Chord = "chord";
        public const string Members = "members";
        public const string Template = "template";
        public const string Edit = "edit";
        public const string Show = "show";
        public const string Replan = "replan";
        public const string Catalog = "catalog";

        public PlanAction(string key, string label)
        {
            Key = key;
            Label = label;
        }

        public string Key { get; }
        public string Label { get; }

        public override string ToString() => Key + " (" + Label + ")";
    }

    /// <summary>
    /// Ronda 8c: lo que la persona lee de cada nudo del plan, en español y en un solo sitio: el estado con icono y color
    /// (verde, ámbar, rojo, gris), si el nudo se ve por defecto en la tabla (los extremos sueltos y las parejas sin cordón
    /// se ocultan y se cuentan en la cabecera), qué hacer con él y la cabecera con la decisión. La ventana, las marcas del
    /// modelo (<c>PlanMarks</c>), la respuesta del MCP (<c>status_text</c>, <c>advice</c>, <c>visible_by_default</c>,
    /// <c>summary_text</c>) y la guía de la IA llaman aquí, así que dicen lo mismo y se prueba sin Revit. El contrato no
    /// cambia: <c>status</c>, <c>orientation</c> y los códigos de aviso siguen en inglés.
    /// Fase 9: los estados <c>created</c> y <c>failed</c> del lote, "ya creada en este lote", el consejo del empalme del
    /// cordón (12.1 de la Fase 8), los botones de cada consejo (<see cref="Actions"/>, mejora C3) y la cabecera con lo creado.
    /// </summary>
    public static class PlanAdvice
    {
        /// <summary>Colores por estado: son los valores de <c>color_name</c> (sin acento para que el JSON sea ASCII).</summary>
        public const string Green = "verde";
        public const string Amber = "ambar";
        public const string Red = "rojo";
        public const string Gray = "gris";

        /// <summary>Leyenda al pie de la ventana.</summary>
        public const string Legend = "Verde = se creará (o creada) · Ámbar = con aviso · Rojo = falta algo o falló · Gris = no se crea";

        /// <summary>Qué hacer cuando el catálogo no tiene ninguna plantilla (todos los nudos salen <c>no_match</c>).</summary>
        public const string CatalogEmptyAdvice = "No hay plantillas: crea primero la conexión de un nudo y guárdala con Guardar en catálogo";

        /// <summary>Ángulo (grados) por debajo del cual una barra del nudo se considera otro tramo del cordón (empalme).</summary>
        public const double SpliceParallelDeg = 5.0;

        /// <summary>RGB de cada color de estado (el mismo en la ventana, en el mapa y en las marcas del modelo).</summary>
        public static (byte R, byte G, byte B) Rgb(string colorName)
        {
            switch (colorName)
            {
                case Green: return (46, 160, 67);
                case Amber: return (240, 160, 0);
                case Red: return (214, 45, 45);
                default: return (140, 140, 140);
            }
        }

        /// <summary>Índice estable del color (<c>color_index</c> del plan): 0 verde, 1 ámbar, 2 rojo, 3 gris.</summary>
        public static int ColorIndexOf(string colorName)
        {
            switch (colorName)
            {
                case Green: return 0;
                case Amber: return 1;
                case Red: return 2;
                default: return 3;
            }
        }

        /// <summary>El nombre del color tal como se escribe en la ventana ("ámbar" con acento).</summary>
        public static string ColorLabel(string colorName) => colorName == Amber ? "ámbar" : colorName;

        /// <summary>Color del estado del nudo: verde (listo o creado), ámbar (con aviso), rojo (falta algo o falló), gris (no se crea).</summary>
        public static string ColorName(PlanNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            switch (node.Status)
            {
                case NodeStatus.Ready:
                case NodeStatus.Created:
                    return node.Warnings.Count > 0 ? Amber : Green;
                case NodeStatus.Invalid:
                case NodeStatus.NoMatch:
                case NodeStatus.AmbiguousChord:
                case NodeStatus.Offset:
                case NodeStatus.Failed:
                    return Red;
                default:
                    return Gray;
            }
        }

        public static int[] ColorRgb(PlanNode node)
        {
            var (r, g, b) = Rgb(ColorName(node));
            return new[] { (int)r, (int)g, (int)b };
        }

        /// <summary>Verdadero si ninguna barra atraviesa el nudo (aviso <c>NODE_CHORD_NOT_CONTINUOUS</c> de la ronda 8b).</summary>
        public static bool ChordMissing(PlanNode node) => node.Warnings.Any(w => w.Code == ErrorCodes.NodeChordNotContinuous);

        /// <summary>Extremo de barra suelto o empalme: no es un nudo.</summary>
        public static bool IsLooseBar(PlanNode node) => node.Status == NodeStatus.Untyped;

        /// <summary>Dos barras que terminan en el mismo punto sin que ninguna pase de largo: falta el cordón en la selección.</summary>
        public static bool IsPairWithoutChord(PlanNode node) =>
            node.Status == NodeStatus.NoMatch && ChordMissing(node) && node.ElementIds.Count <= 2;

        /// <summary>Una barra del nudo casi paralela al cordón: es el otro tramo del mismo cordón (sale por el otro lado).</summary>
        public static bool IsParallelToChord(PlanMember member)
        {
            double angle = Math.Abs(member.AngleDeg);
            return angle <= SpliceParallelDeg || angle >= 180.0 - SpliceParallelDeg;
        }

        /// <summary>
        /// Empalme del cordón (Fase 8, 10.2 y 12.1): ninguna barra atraviesa el nudo, pero el cordón elegido (por el detector
        /// o a mano con Cordón…) termina ahí y <b>otro tramo</b> del mismo cordón sigue por el otro lado como barra del nudo.
        /// No falta ningún cordón en la selección: el cordón está cortado en ese punto, y la típica de tres barras no encaja.
        /// </summary>
        public static bool IsSplice(PlanNode node) =>
            node.Status == NodeStatus.NoMatch && ChordMissing(node) && node.ChordElementId != 0 && node.Members.Any(IsParallelToChord);

        /// <summary>Barras del nudo que no son tramos del cordón (las diagonales y montantes de verdad).</summary>
        public static int DiagonalCount(PlanNode node) => node.Members.Count(m => !IsParallelToChord(m));

        /// <summary>
        /// Visible por defecto en la tabla, en el mapa y en el modelo: los nudos de verdad (con cordón). Las barras sueltas y
        /// las parejas sin cordón se ocultan (decisión P2 de la ronda 8c) y la cabecera las cuenta.
        /// </summary>
        public static bool VisibleByDefault(PlanNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            return !IsLooseBar(node) && !IsPairWithoutChord(node);
        }

        /// <summary>Icono del estado: ● listo, ▲ listo con aviso, ✔ creada, ✖ falta algo o falló, ◌ no se crea, ○ no es nudo.</summary>
        public static string Icon(PlanNode node)
        {
            switch (node.Status)
            {
                case NodeStatus.Ready: return node.Warnings.Count > 0 ? "▲" : "●";
                case NodeStatus.Created: return "✔";
                case NodeStatus.Invalid:
                case NodeStatus.NoMatch:
                case NodeStatus.AmbiguousChord:
                case NodeStatus.Offset:
                case NodeStatus.Failed:
                    return "✖";
                case NodeStatus.Untyped:
                case NodeStatus.Detected:
                    return "○";
                default:
                    return "◌";
            }
        }

        /// <summary>El estado en español, sin icono ("Listo", "Falta el cordón"…). <paramref name="plan"/> distingue "Ya creada en este lote".</summary>
        public static string StatusWord(PlanNode node, BatchPlan? plan = null)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            switch (node.Status)
            {
                case NodeStatus.Ready: return node.Warnings.Count > 0 ? "Listo con aviso" : "Listo";
                case NodeStatus.Created: return node.Warnings.Count > 0 ? "Creada con aviso" : "Creada";
                case NodeStatus.Failed: return "Falló al crear";
                case NodeStatus.Invalid: return "No valida";
                case NodeStatus.NoMatch:
                    if (IsSplice(node)) return "Empalme del cordón";
                    if (ChordMissing(node)) return "Falta el cordón";
                    return IsCatalogEmptyDetail(node) ? "Sin plantillas en el catálogo" : "Sin plantilla que encaje";
                case NodeStatus.AmbiguousChord: return "Dos cordones posibles";
                case NodeStatus.Offset: return "Los ejes no se cortan";
                case NodeStatus.AlreadyConnected: return node.IsCreatedInBatch(plan?.PlanId) ? "Ya creada en este lote" : "Ya tiene conexión";
                case NodeStatus.Excluded: return "Excluido";
                case NodeStatus.Untyped: return "Barra suelta (no es nudo)";
                case NodeStatus.Detected: return "Sin casar";
                default: return node.Status;
            }
        }

        /// <summary>Icono, estado en español y sufijos "(editado)" / "(rehacer)": la columna Estado de la tabla y <c>status_text</c>.</summary>
        public static string StatusText(PlanNode node, BatchPlan? plan = null)
        {
            string text = Icon(node) + " " + StatusWord(node, plan);
            if (node.HasSpecOverride) text += " (editado)";
            if (node.ReplacesExisting && node.Status != NodeStatus.Created) text += " (rehacer)";
            return text;
        }

        /// <summary>Columna Espejo: "no" (igual que la plantilla), "sí" (espejo X), "sí (Y)", "sí (XY)"; vacío si no casó.</summary>
        public static string MirrorText(PlanNode node)
        {
            switch (node.Orientation)
            {
                case "same": return "no";
                case "mirror_x": return "sí";
                case "mirror_y": return "sí (Y)";
                case "both": return "sí (XY)";
                default: return "";
            }
        }

        /// <summary>Globo del mapa y resumen corto: "N4 · Listo · Nudo tipico Detalle D · en espejo".</summary>
        public static string MapLabel(PlanNode node, BatchPlan? plan = null)
        {
            string text = node.Name + " · " + StatusWord(node, plan);
            if (node.TemplateName != null) text += " · " + node.TemplateName;
            if (node.Orientation != null) text += node.IsMirrored ? " · en espejo" : " · igual";
            return text;
        }

        /// <summary>
        /// Qué hacer con el nudo, en una frase (columna "Qué hacer" y <c>advice</c>). Los botones que acompañan a la frase
        /// salen de <see cref="Actions"/>. <paramref name="plan"/> solo hace falta para el catálogo vacío y "ya creada en este lote".
        /// </summary>
        public static string Advice(PlanNode node, BatchPlan? plan = null)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            switch (node.Status)
            {
                case NodeStatus.Ready: return ReadyAdvice(node);
                case NodeStatus.Created: return CreatedAdvice(node);
                case NodeStatus.Failed: return FailedAdvice(node);
                case NodeStatus.Invalid: return InvalidAdvice(node);
                case NodeStatus.NoMatch: return NoMatchAdvice(node, plan);
                case NodeStatus.AmbiguousChord: return "Elige el cordón con Cordón…";
                case NodeStatus.Offset: return OffsetAdvice(node);
                case NodeStatus.AlreadyConnected:
                    return node.IsCreatedInBatch(plan?.PlanId)
                        ? "Creada en este lote (conexión " + ShortId(node.ExistingConnectionId) + "): Borrar el lote la quita; para rehacerla, Incluir (rehacer)"
                        : "Se salta; Incluir (rehacer) replanifica con 'rehacer existentes' para rehacerla con el lote";
                case NodeStatus.Excluded: return "Excluido: Incluir para volver a planificarlo";
                case NodeStatus.Untyped: return "No es un nudo: nada que hacer";
                case NodeStatus.Detected: return "Todavía sin casar: replanifica";
                default: return "—";
            }
        }

        /// <summary>
        /// Los botones de la columna "Qué hacer" para este nudo (mejora C3): uno o dos por caso, con la clave que la ventana
        /// enlaza con la acción del menú de clic derecho (<see cref="PlanAction"/>). Vacío cuando no hay nada que hacer.
        /// </summary>
        public static List<PlanAction> Actions(PlanNode node, BatchPlan? plan = null)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            var actions = new List<PlanAction>();
            void Add(string key, string label) => actions.Add(new PlanAction(key, label));
            switch (node.Status)
            {
                case NodeStatus.Ready:
                    if (node.Warnings.Count > 0) Add(PlanAction.Exclude, "Excluir");
                    if (node.Warnings.Any(w => w.Code == ErrorCodes.TemplateAngleDeviation)) Add(PlanAction.Edit, "Editar nudo");
                    break;
                case NodeStatus.Created:
                    Add(PlanAction.Show, "Ver en Revit");
                    break;
                case NodeStatus.Failed:
                    Add(PlanAction.Show, "Ver en Revit");
                    Add(PlanAction.Exclude, "Excluir");
                    break;
                case NodeStatus.Invalid:
                    if (node.HasSpecOverride && node.Errors.Any(e => e.Code == ErrorCodes.SchemaInvalid)) Add(PlanAction.Edit, "Editar nudo");
                    else Add(PlanAction.Edit, "Editar nudo");
                    Add(PlanAction.Exclude, "Excluir");
                    break;
                case NodeStatus.NoMatch:
                    if ((plan != null && IsCatalogEmpty(plan)) || IsCatalogEmptyDetail(node))
                    {
                        Add(PlanAction.Catalog, "Abrir catálogo");
                    }
                    else if (IsSplice(node))
                    {
                        Add(PlanAction.Exclude, "Excluir");
                    }
                    else if (ChordMissing(node))
                    {
                        Add(PlanAction.Chord, "Cordón…");
                        Add(PlanAction.Members, "Barras…");
                    }
                    else if ((node.StatusDetail ?? "").StartsWith("Sin plantilla por decisión", StringComparison.Ordinal)
                             || (node.StatusDetail ?? "").StartsWith("La plantilla '", StringComparison.Ordinal))
                    {
                        Add(PlanAction.Template, "Plantilla…");
                    }
                    else
                    {
                        Add(PlanAction.Exclude, "Excluir");
                        Add(PlanAction.Template, "Plantilla…");
                    }
                    break;
                case NodeStatus.AmbiguousChord:
                    Add(PlanAction.Chord, "Cordón…");
                    break;
                case NodeStatus.Offset:
                    Add(PlanAction.Show, "Ver en Revit");
                    Add(PlanAction.Exclude, "Excluir");
                    break;
                case NodeStatus.AlreadyConnected:
                    if (node.IsCreatedInBatch(plan?.PlanId)) Add(PlanAction.Show, "Ver en Revit");
                    Add(PlanAction.IncludeReplace, "Incluir (rehacer)");
                    break;
                case NodeStatus.Excluded:
                    Add(PlanAction.Include, "Incluir");
                    break;
                case NodeStatus.Detected:
                    Add(PlanAction.Replan, "Replanificar");
                    break;
            }
            return actions;
        }

        private static string ReadyAdvice(PlanNode node)
        {
            var parts = new List<string>();
            foreach (ApiError warning in node.Warnings)
            {
                switch (warning.Code)
                {
                    case ErrorCodes.TemplateProfileDiffers:
                    {
                        var quoted = Quoted(warning.Message);
                        string subject = (warning.Path ?? "").StartsWith("chord", StringComparison.OrdinalIgnoreCase) ? "El cordón" : "Una barra";
                        parts.Add(quoted.Count >= 2
                            ? subject + " es " + quoted[0] + " y la plantilla " + quoted[1] + ": se creará con la misma cartela; exclúyelo si no quieres"
                            : subject + " tiene otro perfil que la plantilla: se creará con la misma cartela; exclúyelo si no quieres");
                        break;
                    }
                    case ErrorCodes.TemplateAngleDeviation:
                        parts.Add("Una barra se desvía " + Deg(node.MaxDeviationDeg) + " de la plantilla: se creará con el ángulo real; míralo con Editar nudo o exclúyelo");
                        break;
                    case ErrorCodes.NodeChordNotContinuous:
                        parts.Add("Ninguna barra atraviesa el nudo (extremo de cercha o cordón sin seleccionar): se creará igual; si falta el cordón, selecciónalo y replanifica");
                        break;
                    default:
                        parts.Add("Se creará con el aviso " + warning.Code + ": " + Short(warning.Message));
                        break;
                }
            }
            string text = parts.Count == 0 ? "—" : string.Join(" · ", parts.Distinct());
            if (node.ReplacesExisting)
            {
                text = "Rehace la conexión existente " + node.ExistingConnectionId + (text == "—" ? "" : " · " + text);
            }
            return text;
        }

        private static string CreatedAdvice(PlanNode node)
        {
            string text = "Creada: conexión " + ShortId(node.CreatedConnectionId) + " · Ver en Revit; Borrar el lote la quita";
            int warned = node.Warnings.Count(w => w.Code == ErrorCodes.TemplateProfileDiffers || w.Code == ErrorCodes.TemplateAngleDeviation);
            if (warned > 0) text += " · se creó con el aviso del plan (perfil o ángulo distintos de la plantilla)";
            return text;
        }

        private static string FailedAdvice(PlanNode node)
        {
            ApiError? error = node.Errors.LastOrDefault(e => (e.Path ?? "").StartsWith("batch", StringComparison.Ordinal)) ?? node.Errors.LastOrDefault();
            string why = error == null ? "sin detalle" : error.Code + ": " + Short(error.Message);
            string fix = error != null && error.Code == ErrorCodes.ValidationTokenInvalid
                ? "replanifica (el modelo o el plan cambiaron) y pulsa Crear otra vez"
                : "corrige y pulsa Crear otra vez (solo crea los que faltan), o excluye";
            return "Falló al crear (" + why + "): " + fix;
        }

        private static string InvalidAdvice(PlanNode node)
        {
            var codes = node.Errors.Select(e => e.Code).Distinct().ToList();
            if (codes.Contains(ErrorCodes.PlateOutsideGusset)) return "Una barra se sale de la cartela: Editar nudo y agrandarla, o excluir";
            if (codes.Contains(ErrorCodes.ClashWithForeignMember)) return "Choca con una barra ajena: Editar nudo (contorno o espesores) o excluir";
            if (codes.Any(c => c.StartsWith("BOLT_", StringComparison.Ordinal))) return "Los pernos no cumplen los mínimos (" + string.Join(", ", codes) + "): Editar nudo y corregirlos, o excluir";
            if (codes.Contains(ErrorCodes.SchemaInvalid) && node.HasSpecOverride) return "La especificación editada no se puede leer: Quitar edición, o corrígela con Editar nudo";
            if (codes.Contains(ErrorCodes.ProfileMismatch)) return "El perfil escrito no es el del modelo: Editar nudo para corregirlo, o excluir";
            if (codes.Count == 0) return "No valida: Editar nudo para corregirlo, o excluir";
            return "No valida (" + string.Join(", ", codes) + "): Editar nudo para corregirlo, o excluir";
        }

        private static string NoMatchAdvice(PlanNode node, BatchPlan? plan)
        {
            string detail = node.StatusDetail ?? "";
            if ((plan != null && IsCatalogEmpty(plan)) || IsCatalogEmptyDetail(node)) return CatalogEmptyAdvice;
            if (detail.StartsWith("Sin plantilla por decisión", StringComparison.Ordinal)) return "Sin plantilla por decisión tuya: Plantilla… > automática para volver a casarlo";
            if (detail.StartsWith("La plantilla '", StringComparison.Ordinal)) return "La plantilla fijada no está en el plan: Plantilla… para elegir otra";
            if (IsSplice(node))
            {
                int diagonals = DiagonalCount(node);
                return "El cordón termina en este nudo (empalme): ninguna plantilla encaja con " + diagonals + (diagonals == 1 ? " diagonal" : " diagonales") + "; crea esa típica o excluye";
            }
            if (ChordMissing(node)) return "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…";
            string angles = string.Join(", ", node.Members.Select(m => Deg(m.AngleDeg)));
            return "Ninguna plantilla encaja (" + node.Members.Count + (node.Members.Count == 1 ? " barra" : " barras")
                   + (angles.Length > 0 ? ", ángulos " + angles : "") + "): crea esa típica o excluye";
        }

        private static string OffsetAdvice(PlanNode node)
        {
            Match match = Regex.Match(node.StatusDetail ?? "", @"distan ([0-9]+(?:[.,][0-9]+)?) mm");
            return match.Success
                ? "Los ejes se cruzan a " + match.Groups[1].Value + " mm: corrige el modelo o excluye"
                : "Los ejes no se cortan: corrige el modelo o excluye";
        }

        private static bool IsCatalogEmptyDetail(PlanNode node) => (node.StatusDetail ?? "").StartsWith("No hay plantillas", StringComparison.Ordinal);

        private static List<string> Quoted(string text) => Regex.Matches(text ?? "", "'([^']*)'").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

        private static string Deg(double? value) => value.HasValue ? value.Value.ToString("0.#", CultureInfo.InvariantCulture) + "°" : "?°";

        private static string Short(string message)
        {
            string text = (message ?? "").Trim().TrimEnd('.');
            return text.Length <= 110 ? text : text.Substring(0, 107) + "…";
        }

        private static string ShortId(string? id) => string.IsNullOrEmpty(id) ? "?" : id!.Length > 8 ? id.Substring(0, 8) + "…" : id;

        // ---- plan entero ----

        /// <summary>Verdadero si el plan se calculó sin ninguna plantilla (catálogo vacío o todas ilegibles).</summary>
        public static bool IsCatalogEmpty(BatchPlan plan) => plan != null && plan.Templates.Count == 0;

        /// <summary>Aviso <c>CATALOG_EMPTY</c> para la respuesta y la ventana.</summary>
        public static ApiError CatalogEmptyWarning() => new ApiError(ErrorCodes.CatalogEmpty,
            CatalogEmptyAdvice + " (o con conn_catalog_save). Con el catálogo vacío ningún nudo puede casar: todos salen no_match.",
            "template_ids", "Botón Catálogo > Guardar en catálogo desde una conexión del modelo, o conn_catalog_save desde la IA.");

        public static int VisibleCount(BatchPlan plan) => plan.Nodes.Count(VisibleByDefault);

        public static int HiddenPairsWithoutChord(BatchPlan plan) => plan.Nodes.Count(IsPairWithoutChord);

        public static int HiddenLooseBars(BatchPlan plan) => plan.Nodes.Count(IsLooseBar);

        /// <summary>"20 sin cordón, 23 barras sueltas" (vacío si no hay ocultos).</summary>
        public static string HiddenText(BatchPlan plan)
        {
            var parts = new List<string>();
            int pairs = HiddenPairsWithoutChord(plan);
            int loose = HiddenLooseBars(plan);
            if (pairs > 0) parts.Add(pairs + " sin cordón");
            if (loose > 0) parts.Add(loose + (loose == 1 ? " barra suelta" : " barras sueltas"));
            return string.Join(", ", parts);
        }

        /// <summary>Texto del botón Crear: "Crear 16 conexiones" / "Crear 1 conexión" / "Crear 2 conexiones (1 reintento)".</summary>
        public static string CreateButtonText(BatchPlan plan)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            int total = plan.CreatableCount;
            int retry = plan.Nodes.Count(n => n.Status == NodeStatus.Failed && BatchRunner.CanBeCreated(n));
            string text = total == 1 ? "Crear 1 conexión" : "Crear " + total + " conexiones";
            if (retry > 0) text += " (" + retry + (retry == 1 ? " reintento)" : " reintentos)");
            return text;
        }

        /// <summary>
        /// La cabecera con la decisión (<c>summary_text</c>): "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales,
        /// 8 en espejo). 14 avisan de perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas." Con 0 listos:
        /// "Ningún nudo listo: …" y la causa más frecuente. Fase 9, después del lote: "Creadas 15 conexiones (14 con aviso),
        /// 1 falló. Quedan 0 listas sin crear. …"; tras replanificar: "16 ya creadas en este lote (Borrar el lote las quita). …".
        /// </summary>
        public static string SummaryText(BatchPlan plan)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var ready = plan.Nodes.Where(n => n.Status == NodeStatus.Ready).ToList();
            var created = plan.Nodes.Where(n => n.Status == NodeStatus.Created).ToList();
            var failed = plan.Nodes.Where(n => n.Status == NodeStatus.Failed).ToList();
            var visible = plan.Nodes.Where(VisibleByDefault).ToList();
            int invalid = visible.Count(n => n.Status == NodeStatus.Invalid);
            int splice = visible.Count(IsSplice);
            int noMatch = visible.Count(n => n.Status == NodeStatus.NoMatch && !ChordMissing(n));
            int chordMissing = visible.Count(n => n.Status == NodeStatus.NoMatch && ChordMissing(n) && !IsSplice(n));
            int ambiguous = visible.Count(n => n.Status == NodeStatus.AmbiguousChord);
            int offset = visible.Count(n => n.Status == NodeStatus.Offset);
            int inBatch = visible.Count(n => n.IsCreatedInBatch(plan.PlanId));
            int already = visible.Count(n => n.Status == NodeStatus.AlreadyConnected) - inBatch;
            int excluded = visible.Count(n => n.Status == NodeStatus.Excluded);

            var sentences = new List<string>();
            string skip = "";
            if (created.Count > 0 || failed.Count > 0)
            {
                int warned = created.Count(n => n.Warnings.Count > 0);
                string text = created.Count == 1 ? "Creada 1 conexión" : "Creadas " + created.Count + " conexiones";
                if (warned > 0) text += " (" + warned + " con aviso)";
                if (failed.Count > 0) text += ", " + failed.Count + (failed.Count == 1 ? " falló" : " fallaron");
                sentences.Add(text + ".");
                if (ready.Count > 0) sentences.Add("Quedan " + ready.Count + (ready.Count == 1 ? " lista sin crear." : " listas sin crear."));
                else if (failed.Count > 0) sentences.Add("Pulsa Crear otra vez para reintentar " + (failed.Count == 1 ? "la fallida." : "las fallidas."));
            }
            else if (ready.Count > 0)
            {
                int same = ready.Count(n => !n.IsMirrored);
                int mirrored = ready.Count - same;
                var byTemplate = ready.GroupBy(n => n.TemplateName ?? n.TemplateId ?? "especificación editada")
                    .OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal).ToList();
                string with = byTemplate.Count == 1
                    ? "con " + byTemplate[0].Key
                    : "con " + byTemplate.Count + " plantillas (" + string.Join(", ", byTemplate.Select(g => g.Key + ": " + g.Count())) + ")";
                sentences.Add((ready.Count == 1 ? "Se creará 1 conexión " : "Se crearán " + ready.Count + " conexiones ") + with
                              + " (" + same + (same == 1 ? " igual, " : " iguales, ") + mirrored + " en espejo).");
                int profile = ready.Count(n => n.Warnings.Any(w => w.Code == ErrorCodes.TemplateProfileDiffers));
                int angle = ready.Count(n => n.Warnings.Any(w => w.Code == ErrorCodes.TemplateAngleDeviation));
                int other = ready.Count(n => n.Warnings.Any(w => w.Code != ErrorCodes.TemplateProfileDiffers && w.Code != ErrorCodes.TemplateAngleDeviation));
                if (profile > 0) sentences.Add(profile + (profile == 1 ? " avisa" : " avisan") + " de perfil distinto.");
                if (angle > 0) sentences.Add(angle + (angle == 1 ? " avisa" : " avisan") + " de desvío de ángulo.");
                if (other > 0) sentences.Add(other + " con otros avisos.");
            }
            else if (inBatch > 0)
            {
                sentences.Add("Nada que crear: " + inBatch + (inBatch == 1 ? " conexión ya creada en este lote" : " conexiones ya creadas en este lote") + " (Borrar el lote las quita).");
                skip = "in_batch";
            }
            else
            {
                var (cause, key) = NoReadyCause(plan, visible, invalid, noMatch, chordMissing, splice, ambiguous, offset, already, excluded, inBatch);
                sentences.Add("Ningún nudo listo: " + cause + ".");
                skip = key;
            }
            if (inBatch > 0 && skip != "in_batch") sentences.Add(inBatch + (inBatch == 1 ? " ya creada en este lote" : " ya creadas en este lote") + " (Borrar el lote las quita).");
            if (invalid > 0 && skip != "invalid") sentences.Add(invalid + (invalid == 1 ? " nudo no valida." : " nudos no validan."));
            if (noMatch > 0 && skip != "no_match") sentences.Add(noMatch + " sin plantilla que encaje.");
            if (splice > 0 && skip != "splice") sentences.Add(splice + (splice == 1 ? " empalme del cordón (sin plantilla)." : " empalmes del cordón (sin plantilla)."));
            if (chordMissing > 0 && skip != "chord") sentences.Add(chordMissing + " con el cordón sin seleccionar.");
            if (ambiguous > 0 && skip != "ambiguous") sentences.Add(ambiguous + " con dos cordones posibles.");
            if (offset > 0 && skip != "offset") sentences.Add(offset + " con ejes que no se cortan.");
            if (already > 0 && skip != "already") sentences.Add(already + (already == 1 ? " ya tiene conexión." : " ya tienen conexión."));
            if (excluded > 0 && skip != "excluded") sentences.Add(excluded + (excluded == 1 ? " excluido." : " excluidos."));
            string hidden = HiddenText(plan);
            if (hidden.Length > 0) sentences.Add("Ocultos: " + hidden + ".");
            return string.Join(" ", sentences);
        }

        private static (string Cause, string Key) NoReadyCause(BatchPlan plan, List<PlanNode> visible, int invalid, int noMatch, int chordMissing, int splice,
            int ambiguous, int offset, int already, int excluded, int inBatch)
        {
            if (IsCatalogEmpty(plan)) return ("no hay plantillas en el catálogo (crea primero la conexión de un nudo y guárdala con Guardar en catálogo)", "catalog");
            if (plan.Nodes.Count == 0) return ("no se detectó ningún nudo en la selección", "");
            if (visible.Count == 0) return ("la selección solo tiene barras sueltas o parejas sin cordón (selecciona también los cordones y replanifica)", "");
            var causes = new List<(int Count, string Key, string Text)>
            {
                (noMatch, "no_match", "ninguna plantilla encaja en " + noMatch + (noMatch == 1 ? " nudo" : " nudos")),
                (invalid, "invalid", invalid + (invalid == 1 ? " nudo no valida" : " nudos no validan")),
                (chordMissing, "chord", "falta el cordón en " + chordMissing + (chordMissing == 1 ? " nudo" : " nudos")),
                (splice, "splice", splice + (splice == 1 ? " empalme del cordón sin plantilla" : " empalmes del cordón sin plantilla")),
                (ambiguous, "ambiguous", ambiguous + (ambiguous == 1 ? " nudo con dos cordones posibles" : " nudos con dos cordones posibles")),
                (offset, "offset", offset + (offset == 1 ? " nudo con ejes que no se cortan" : " nudos con ejes que no se cortan")),
                (already, "already", already + (already == 1 ? " nudo ya tiene conexión" : " nudos ya tienen conexión")),
                (inBatch, "in_batch", inBatch + (inBatch == 1 ? " conexión ya creada en este lote" : " conexiones ya creadas en este lote")),
                (excluded, "excluded", excluded + (excluded == 1 ? " nudo excluido" : " nudos excluidos")),
            };
            var best = causes.Where(c => c.Count > 0).OrderByDescending(c => c.Count).FirstOrDefault();
            return best.Count > 0 ? (best.Text, best.Key) : ("ningún nudo casa con las plantillas", "");
        }

        /// <summary>
        /// Pone en cada nudo el color de su estado (<c>color_name</c>, <c>color_rgb</c>, <c>color_index</c>) en vez del color
        /// por nudo de la paleta: lo llama el add-in después de <c>PlanBuilder.Build</c> para que el plan en memoria, su JSON,
        /// las marcas y la ventana digan lo mismo (decisión P4 de la ronda 8c).
        /// </summary>
        public static void ApplyStatusColors(BatchPlan plan)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            foreach (PlanNode node in plan.Nodes)
            {
                string color = ColorName(node);
                node.ColorName = color;
                node.ColorRgb = ColorRgb(node);
                node.ColorIndex = ColorIndexOf(color);
            }
        }
    }
}
