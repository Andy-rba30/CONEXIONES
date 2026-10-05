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
    }
}
