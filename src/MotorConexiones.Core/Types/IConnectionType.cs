namespace MotorConexiones.Core.Types
{
    /// <summary>
    /// Un tipo de conexión = un archivo en <c>Core/Types/</c>. Registra su nombre, su esquema, su ejemplo y sus
    /// validaciones propias sin tocar el núcleo. En la Fase 2 se amplía con el esquema y la validación;
    /// en la Fase 1 solo existe el registro.
    /// </summary>
    public interface IConnectionType
    {
        /// <summary>Nombre estable que va en <c>connection_type</c> (por ejemplo <c>gusset_node</c>).</summary>
        string Name { get; }

        /// <summary>Descripción corta en español: qué es y cuándo usarlo (la ve la IA en <c>conn_list_types</c>).</summary>
        string Description { get; }
    }
}
