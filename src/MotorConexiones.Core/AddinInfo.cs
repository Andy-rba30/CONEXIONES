namespace MotorConexiones.Core
{
    /// <summary>Datos fijos del add-in que se muestran en <c>meta</c> y en <c>conn_ping</c>.</summary>
    public static class AddinInfo
    {
        /// <summary>Versión del add-in (se copia en <c>meta.addin_version</c>).</summary>
        public const string Version = "0.8.3";

        /// <summary>Versión del contrato JSON que entiende este add-in.</summary>
        public const string SpecVersion = "1.0";

        /// <summary>Nombre que llevan los elementos y los registros creados por el add-in.</summary>
        public const string Name = "MotorConexiones";
    }
}
