using System.ComponentModel;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;

namespace MotorConexiones.Revit.UI
{
    /// <summary>Fila de la tabla editable (enlazada al DataGrid de la ventana).</summary>
    public sealed class FieldRow : INotifyPropertyChanged
    {
        private string _value;

        public FieldRow(SpecField field)
        {
            Path = field.Path;
            Group = field.Group;
            Label = field.Label;
            _value = field.Value;
            IsReadOnly = field.IsReadOnly;
            Hint = BuildHint(field);
        }

        public string Path { get; }
        public string Group { get; }
        public string Label { get; }
        public bool IsReadOnly { get; }
        public string Hint { get; }

        public string Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private static string BuildHint(SpecField field)
        {
            string hint = field.Hint ?? "";
            if (field.Options != null && field.Options.Count > 0)
            {
                hint = (hint.Length > 0 ? hint + ". " : "") + "Valores: " + string.Join(", ", field.Options);
            }
            switch (field.Kind)
            {
                case SpecFieldKind.Boolean:
                    hint = (hint.Length > 0 ? hint + ". " : "") + "sí / no";
                    break;
                case SpecFieldKind.NumberList:
                    hint = (hint.Length > 0 ? hint + ". " : "") + "Ejemplo: 75; 420; 70";
                    break;
                case SpecFieldKind.PointPair:
                    hint = (hint.Length > 0 ? hint + ". " : "") + "Ejemplo: -175; 280";
                    break;
            }
            return hint;
        }
    }

    /// <summary>Fila de la lista de errores y avisos (los mismos que recibe la IA).</summary>
    public sealed class MessageRow
    {
        public MessageRow(string level, ApiError error)
        {
            Level = level;
            Code = error.Code;
            Path = error.Path ?? "";
            Message = error.Message;
            Hint = error.Hint ?? "";
        }

        public string Level { get; }
        public string Code { get; }
        public string Path { get; }
        public string Message { get; }
        public string Hint { get; }
    }
}
