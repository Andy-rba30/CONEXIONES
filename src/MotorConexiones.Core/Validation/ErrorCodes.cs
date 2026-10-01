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

        // Fabricación
        public const string FabricationFailed = "FABRICATION_FAILED";
        public const string CategoryFallback = "CATEGORY_FALLBACK";
    }
}
