using System;
using System.Collections.Generic;
using System.Linq;

namespace MotorConexiones.Core.Types
{
    /// <summary>Registro de tipos de conexión disponibles. Los tipos se añaden como archivos nuevos y se registran aquí.</summary>
    public static class ConnectionTypeRegistry
    {
        private static readonly object Gate = new object();
        private static readonly Dictionary<string, IConnectionType> Types = new Dictionary<string, IConnectionType>(StringComparer.Ordinal);

        public static void Register(IConnectionType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrWhiteSpace(type.Name)) throw new ArgumentException("El tipo de conexión necesita un nombre.", nameof(type));
            lock (Gate)
            {
                Types[type.Name] = type;
            }
        }

        public static IReadOnlyList<IConnectionType> All
        {
            get
            {
                lock (Gate)
                {
                    return Types.Values.OrderBy(t => t.Name, StringComparer.Ordinal).ToList();
                }
            }
        }

        public static IConnectionType? Find(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            lock (Gate)
            {
                return Types.TryGetValue(name, out var type) ? type : null;
            }
        }

        /// <summary>Solo para pruebas.</summary>
        public static void Clear()
        {
            lock (Gate)
            {
                Types.Clear();
            }
        }
    }
}
