using System.Collections.Generic;

namespace MotorConexiones.Core.Editing
{
    /// <summary>Cómo se edita un campo de la tabla y cómo se interpreta el texto escrito.</summary>
    public enum SpecFieldKind
    {
        /// <summary>Texto libre.</summary>
        Text,
        /// <summary>Número decimal en mm o grados; admite coma o punto decimal.</summary>
        Number,
        /// <summary>Entero (filas, columnas, ElementId).</summary>
        Integer,
        /// <summary>sí / no.</summary>
        Boolean,
        /// <summary>Uno de <see cref="SpecField.Options"/>.</summary>
        Choice,
        /// <summary>Lista de números separados por punto y coma: <c>75; 420; 70</c>.</summary>
        NumberList,
        /// <summary>Par de coordenadas en mm: <c>-175; 280</c>.</summary>
        PointPair,
        /// <summary>Solo lectura (dato del modelo o de la especificación que no se edita aquí).</summary>
        ReadOnly,
    }

    /// <summary>Una fila de la tabla editable de la ventana: ruta JSON, grupo, etiqueta en español y valor como texto.</summary>
    public sealed class SpecField
    {
        public SpecField(string path, string group, string label, string value, SpecFieldKind kind, IReadOnlyList<string>? options = null, string? hint = null)
        {
            Path = path;
            Group = group;
            Label = label;
            Value = value ?? "";
            Kind = kind;
            Options = options;
            Hint = hint;
        }

        /// <summary>Ruta JSON del campo, la misma que usan los errores del validador y las cotas del croquis.</summary>
        public string Path { get; }

        /// <summary>Grupo de la tabla: Cordón, Cartela, Barra 1, Dudas…</summary>
        public string Group { get; }

        /// <summary>Nombre en español que ve la persona.</summary>
        public string Label { get; }

        /// <summary>Valor actual formateado (números con coma decimal, booleanos "sí"/"no").</summary>
        public string Value { get; }

        public SpecFieldKind Kind { get; }

        /// <summary>Valores admitidos cuando <see cref="Kind"/> es <see cref="SpecFieldKind.Choice"/>.</summary>
        public IReadOnlyList<string>? Options { get; }

        /// <summary>Ayuda corta (qué significa, qué formato se espera).</summary>
        public string? Hint { get; }

        public bool IsReadOnly => Kind == SpecFieldKind.ReadOnly;
    }

    /// <summary>Resultado de aplicar un texto a un campo.</summary>
    public sealed class SpecEditResult
    {
        private SpecEditResult(bool ok, string? error, string? note)
        {
            Ok = ok;
            Error = error;
            Note = note;
        }

        public bool Ok { get; }

        /// <summary>Mensaje en español cuando el texto no se pudo interpretar o la ruta no es editable.</summary>
        public string? Error { get; }

        /// <summary>Aviso informativo cuando el editor también cambió otro campo (por ejemplo la etiqueta del espesor).</summary>
        public string? Note { get; }

        public static SpecEditResult Success(string? note = null) => new SpecEditResult(true, null, note);
        public static SpecEditResult Failure(string error) => new SpecEditResult(false, error, null);
    }
}
