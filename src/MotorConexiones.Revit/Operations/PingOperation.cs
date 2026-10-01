using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Autodesk.Revit.DB;
using MotorConexiones.Core;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Revit.Operations
{
    /// <summary><c>conn_ping</c>: el add-in está cargado, versión, versión de Revit y backend activo. No toca el modelo.</summary>
    public sealed class PingOperation : IOperation
    {
        public string Name => "ping";
        public bool RequiresDocument => false;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document? doc = context.Document;
            Autodesk.Revit.ApplicationServices.Application? app = doc?.Application ?? context.UIApplication?.Application;
            Assembly assembly = typeof(Bridge).Assembly;

            object? document = null;
            if (doc != null)
            {
                document = new
                {
                    title = doc.Title,
                    path = doc.PathName,
                    is_family = doc.IsFamilyDocument,
                    is_workshared = doc.IsWorkshared,
                    is_modifiable = doc.IsModifiable,
                    is_read_only = doc.IsReadOnly,
                };
            }

            var data = new
            {
                addin_version = AddinInfo.Version,
                spec_version = AddinInfo.SpecVersion,
                backend = "directshape (camino B, provisional hasta la decisión de la Fase 1)",
                operations = Bridge.OperationNames,
                revit = app == null ? null : new
                {
                    version_number = app.VersionNumber,
                    version_build = app.VersionBuild,
                    version_name = app.VersionName,
                    sub_version_number = app.SubVersionNumber,
                    language = app.Language.ToString(),
                },
                dotnet = new
                {
                    framework = RuntimeInformation.FrameworkDescription,
                    assembly_location = assembly.Location,
                    load_context = AssemblyLoadContext.GetLoadContext(assembly)?.Name ?? "?",
                },
                document,
                has_uidocument = context.UIDocument != null,
            };
            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
