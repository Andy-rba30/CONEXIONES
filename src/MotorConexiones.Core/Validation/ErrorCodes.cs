namespace MotorConexiones.Core.Validation
{
    /// <summary>
    /// Códigos estables de error y advertencia. Los de la sección 8 del encargo se completan en la Fase 2;
    /// aquí están los que ya usa el puente y la prueba técnica de la Fase 1.
    /// </summary>
    public static class ErrorCodes
    {
        // Puente y transacciones
        public const string UnknownOperation = "UNKNOWN_OPERATION";
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string NoDocument = "NO_DOCUMENT";
        public const string RevitBusy = "REVIT_BUSY";
        public const string InternalError = "INTERNAL_ERROR";
        public const string RevitDialogSuppressed = "REVIT_DIALOG_SUPPRESSED";
        public const string RevitWarning = "REVIT_WARNING";
        public const string RevitError = "REVIT_ERROR";
        public const string TransactionFailed = "TRANSACTION_FAILED";

        // Modelo (sección 8.10)
        public const string ElementNotFound = "ELEMENT_NOT_FOUND";
        public const string ElementNotAMember = "ELEMENT_NOT_A_MEMBER";
        public const string MemberNotAtNode = "MEMBER_NOT_AT_NODE";
        /// <summary>Fase 12 (aviso): el eje de la barra corta el cordón a menos de medio canto del punto de trabajo (nudo en K con excentricidad).</summary>
        public const string MemberEccentric = "MEMBER_ECCENTRIC";
        /// <summary>Fase 12 (aviso, direct_weld): la huella cortada de la barra sobresale del ancho de la cara del cordón.</summary>
        public const string MemberWiderThanChordFace = "MEMBER_WIDER_THAN_CHORD_FACE";
        /// <summary>Fase 12 (aviso, direct_weld): las huellas de dos barras se pisan sobre la cara del cordón.</summary>
        public const string MembersOverlapOnFace = "MEMBERS_OVERLAP_ON_FACE";
        /// <summary>Fase 12 (aviso, direct_weld): el corte inclinado contra la cara no se pudo hacer y se usó el retiro por el eje.</summary>
        public const string EndCutApproximated = "END_CUT_APPROXIMATED";
        /// <summary>Fase 12 (aviso, end_plate): la API no ofrece la tuerca a ambos lados; perno estándar (decisión D9).</summary>
        public const string BoltNutsApproximated = "BOLT_NUTS_APPROXIMATED";
        /// <summary>Fase 12 (error, end_plate): un perno queda a menos del mínimo de la cara del tubo.</summary>
        public const string BoltTooCloseToTube = "BOLT_TOO_CLOSE_TO_TUBE";
        /// <summary>Fase 12 (error, end_plate): el agujero no es mayor que el perno.</summary>
        public const string BoltHoleTooSmall = "BOLT_HOLE_TOO_SMALL";
        /// <summary>Fase 12 (error, end_plate): los dos tramos no son colineales o no son del mismo perfil (aviso en este último caso).</summary>
        public const string SpliceNotCollinear = "SPLICE_NOT_COLLINEAR";
        /// <summary>Fase 12 (aviso, plan): el nudo en K está en un empalme del cordón; cada diagonal se suelda al tramo al que llega.</summary>
        public const string NodeAtSplice = "NODE_AT_SPLICE";
        /// <summary>Fase 12 (aviso, end_plate): los dos tramos del empalme son de perfiles distintos.</summary>
        public const string SpliceProfilesDiffer = "SPLICE_PROFILES_DIFFER";
        /// <summary>Fase 12 (error, end_plate): la plancha no cubre la huella del tubo.</summary>
        public const string PlateSmallerThanTube = "PLATE_SMALLER_THAN_TUBE";
        /// <summary>Fase 12 (error, end_plate): el retiro de un tramo no deja sitio a su plancha (espesor más media holgura).</summary>
        public const string SpliceSetbackTooSmall = "SPLICE_SETBACK_TOO_SMALL";

        // Nudo (sección 7)
        public const string NodeAxesNotIntersecting = "NODE_AXES_NOT_INTERSECTING";
        public const string NodeAxesParallel = "NODE_AXES_PARALLEL";
        public const string NodeNeedsTwoMembers = "NODE_NEEDS_TWO_MEMBERS";
        /// <summary>Aviso del plan (ronda 8b): ninguna barra atraviesa el nudo; el cordón es la más horizontal de las que llegan.</summary>
        public const string NodeChordNotContinuous = "NODE_CHORD_NOT_CONTINUOUS";

        // Fabricación
        public const string FabricationFailed = "FABRICATION_FAILED";
        public const string CategoryFallback = "CATEGORY_FALLBACK";

        // Esquema y validación v1 (sección 8 del encargo)
        public const string SchemaInvalid = "SCHEMA_INVALID";
        public const string UnresolvedUncertainty = "UNRESOLVED_UNCERTAINTY";
        public const string DimensionChainMismatch = "DIMENSION_CHAIN_MISMATCH";
        public const string LabelValueMismatch = "LABEL_VALUE_MISMATCH";
        public const string ProfileMismatch = "PROFILE_MISMATCH";
        public const string AngleDiffersFromModel = "ANGLE_DIFFERS_FROM_MODEL";
        public const string BoltEdgeDistanceTooSmall = "BOLT_EDGE_DISTANCE_TOO_SMALL";
        public const string BoltSpacingTooSmall = "BOLT_SPACING_TOO_SMALL";
        public const string BoltOutsidePlate = "BOLT_OUTSIDE_PLATE";
        /// <summary>Advertencia (ronda 6b): bolts.length_mm menor que agarre + suplemento (tuerca, arandela y rosca).</summary>
        public const string BoltLengthTooShort = "BOLT_LENGTH_TOO_SHORT";
        public const string WeldBelowMinimum = "WELD_BELOW_MINIMUM";
        public const string OutlineInvalid = "OUTLINE_INVALID";
        public const string PlateOutsideGusset = "PLATE_OUTSIDE_GUSSET";
        public const string ClashWithForeignMember = "CLASH_WITH_FOREIGN_MEMBER";
        public const string ValidationTokenInvalid = "VALIDATION_TOKEN_INVALID";
        /// <summary>Fase 13 (aviso): la cartela automática sale grande (caja mayor que max_auto_gusset_mm o área más del doble de lo que cubre).</summary>
        public const string OutlineAutoLarge = "OUTLINE_AUTO_LARGE";

        // Fase 13: partir una barra (conn_split_member / conn_unsplit)
        /// <summary>El corte no se puede hacer o deshacer: una conexión del add-in usa la barra (o los tramos).</summary>
        public const string SplitInUse = "SPLIT_IN_USE";
        /// <summary>Revit no pudo partir la barra (copiar el elemento, cambiar las curvas); nada cambió.</summary>
        public const string SplitFailed = "SPLIT_FAILED";
        /// <summary>Aviso: el tramo nuevo no conserva algún parámetro del original (giro de sección, justificación, desfases) y se escribió a mano o quedó distinto.</summary>
        public const string SplitParametersApproximated = "SPLIT_PARAMETERS_APPROXIMATED";
        /// <summary>Aviso: al deshacer el corte la barra no volvió exactamente a como estaba (curva o extensiones); la barra original nunca se borra.</summary>
        public const string SplitUndoPartial = "SPLIT_UNDO_PARTIAL";
        /// <summary>No hay ningún corte registrado para esa barra o ese split_id.</summary>
        public const string SplitNotFound = "SPLIT_NOT_FOUND";

        // Catálogo de plantillas (Fase 7)
        /// <summary>No existe ninguna plantilla con ese template_id (o nombre) en la carpeta del catálogo.</summary>
        public const string TemplateNotFound = "TEMPLATE_NOT_FOUND";
        /// <summary>Ya hay una plantilla con ese nombre y no se pidió overwrite: true.</summary>
        public const string TemplateExists = "TEMPLATE_EXISTS";
        /// <summary>El archivo de plantilla no se puede leer o no tiene la estructura esperada.</summary>
        public const string TemplateInvalid = "TEMPLATE_INVALID";
        /// <summary>La especificación que se quería guardar como plantilla no valida o no describe el nudo.</summary>
        public const string TemplateSpecInvalid = "TEMPLATE_SPEC_INVALID";
        /// <summary>La especificación tiene dudas (uncertain_fields) sin confirmar: una plantilla no puede arrastrarlas.</summary>
        public const string TemplateHasOpenUncertainties = "TEMPLATE_HAS_OPEN_UNCERTAINTIES";
        /// <summary>Ninguna de las orientaciones probadas casa todas las ranuras de la plantilla con las barras del nudo.</summary>
        public const string TemplateNoMatch = "TEMPLATE_NO_MATCH";
        /// <summary>Advertencia: una barra se desvía de la plantilla más de lo configurado (config/catalog.json).</summary>
        public const string TemplateAngleDeviation = "TEMPLATE_ANGLE_DEVIATION";
        /// <summary>Advertencia: el perfil del nudo no es el de la plantilla (profile_policy warn); se escribe el del modelo.</summary>
        public const string TemplateProfileDiffers = "TEMPLATE_PROFILE_DIFFERS";
        /// <summary>La carpeta del catálogo no existe y no se pudo crear.</summary>
        public const string CatalogFolderUnavailable = "CATALOG_FOLDER_UNAVAILABLE";

        // Plan de lote (Fase 8)
        /// <summary>No hay ningún plan con ese plan_id en memoria (se descartó o Revit se reinició).</summary>
        public const string PlanNotFound = "PLAN_NOT_FOUND";
        /// <summary>Advertencia: la vista activa no admite colores por elemento; el plan se calculó sin marcas.</summary>
        public const string PlanMarksSkipped = "PLAN_MARKS_SKIPPED";
        /// <summary>Advertencia (cierre de la Fase 8): al marcar un plan se quitaron las marcas de otro plan del mismo documento; en una vista solo se marca un plan.</summary>
        public const string PlanMarksReplaced = "PLAN_MARKS_REPLACED";
        /// <summary>Advertencia (ronda 8c): el catálogo no tiene ninguna plantilla, así que ningún nudo puede casar (todos salen no_match).</summary>
        public const string CatalogEmpty = "CATALOG_EMPTY";

        // Crear por lotes (Fase 9)
        /// <summary>Con stop_on_error, un nudo falló y el lote entero se revirtió (el informe va en data).</summary>
        public const string BatchStopped = "BATCH_STOPPED";
        /// <summary>Advertencia: un nudo del lote falló y se revirtió solo; los demás siguen (el informe dice cuál y por qué).</summary>
        public const string BatchNodeFailed = "BATCH_NODE_FAILED";
        /// <summary>Advertencia: no hay ninguna conexión con ese batch_id en el modelo (nada que borrar).</summary>
        public const string BatchEmpty = "BATCH_EMPTY";
        /// <summary>Advertencia: el grupo exterior del lote no se pudo abrir o asimilar; el lote quedó con una entrada de deshacer por nudo.</summary>
        public const string BatchUndoSplit = "BATCH_UNDO_SPLIT";

        // Fase 10 (selección asistida y etiquetas pinchables)
        /// <summary>Advertencia: la selección asistida añadió barras a la selección (el plan se calculó con todas); el mensaje dice cuántas y por qué.</summary>
        public const string SelectionExpanded = "SELECTION_EXPANDED";
        /// <summary>Advertencia: no se pudieron poner (o quitar) las etiquetas del lienzo; el plan sigue con colores y marcadores.</summary>
        public const string PlanLabelsSkipped = "PLAN_LABELS_SKIPPED";
    }
}
