using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Revit.Catalog;

namespace MotorConexiones.Revit.Operations
{
    /// <summary><c>conn_catalog_list</c>: las plantillas del catálogo (nombre, id, tipo, etiquetas, barras, cordón, fecha), sin volcarlas.</summary>
    public sealed class CatalogListOperation : IOperation
    {
        public string Name => "catalog_list";
        public bool RequiresDocument => false;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            CatalogConfig config = CatalogConfigLoader.Load();
            CatalogStore store;
            try
            {
                store = CatalogConfigLoader.OpenStore(config);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }

            var warnings = new List<ApiError>(context.Warnings);
            IReadOnlyList<CatalogEntry> entries = store.List(warnings);
            var data = new
            {
                catalog_folder = store.Folder,
                shared_catalog_folder = CatalogConfigLoader.ResolveSharedFolder(config),
                templates_count = entries.Count,
                templates = entries.Select(CatalogService.EntryToData).ToList(),
            };
            return ApiResponse.Success(Name, data, warnings);
        }
    }
}
