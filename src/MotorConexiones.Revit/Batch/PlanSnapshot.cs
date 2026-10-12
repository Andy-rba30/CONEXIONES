using System;
using System.Collections.Generic;
using MotorConexiones.Core.Batch;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>
    /// Lo que la ventana no modal necesita del modelo, leído de una vez en contexto válido de la API (cierre de la ronda
    /// 8c): el plan, el alzado de la cercha para el mapa y el nombre del tipo de cada barra (para las listas de Cordón… y
    /// Barras…). La ventana no vuelve a leer el modelo por su cuenta; recibe un <see cref="PlanSnapshot"/> nuevo después de
    /// cada acción que pasa por <see cref="PlanEvents"/>.
    /// </summary>
    public sealed class PlanSnapshot
    {
        public PlanSnapshot(BatchPlan plan, TrussMap? map, IReadOnlyDictionary<long, string> typeNames, IReadOnlyDictionary<string, string?>? thumbnailPaths = null)
        {
            Plan = plan ?? throw new ArgumentNullException(nameof(plan));
            Map = map;
            TypeNames = typeNames ?? new Dictionary<long, string>();
            ThumbnailPaths = thumbnailPaths ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Fase 11 (V4): template_id → ruta del PNG de su miniatura (nulo si no se pudo dibujar), para el detalle del nudo y Plantilla….</summary>
        public IReadOnlyDictionary<string, string?> ThumbnailPaths { get; }

        public BatchPlan Plan { get; }

        /// <summary>El alzado de la cercha, o nulo si no se pudo calcular (la ventana lo dice en el log, no falla).</summary>
        public TrussMap? Map { get; }

        /// <summary>Id de barra → nombre del tipo (por ejemplo "HSS3X3X1/4"); solo decorativo.</summary>
        public IReadOnlyDictionary<long, string> TypeNames { get; }
    }
}
