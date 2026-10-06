using System;
using System.Runtime.InteropServices;
using Autodesk.Revit.UI;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Activa la ventana principal de Revit (cierre de la ronda 8d). Antes de pinchar en el modelo desde la ventana no modal
    /// del plan, la ventana se oculta y el foco tiene que volver a Revit: la API da el manejador de su ventana
    /// (<see cref="UIApplication.MainWindowHandle"/>) pero no un método para activarla, así que se usa <c>user32</c>.
    /// Nunca lanza: devuelve falso si no se pudo.
    /// </summary>
    internal static class RevitMainWindow
    {
        public static bool Activate(UIApplication app)
        {
            if (app == null) return false;
            try
            {
                IntPtr handle = app.MainWindowHandle;
                return handle != IntPtr.Zero && SetForegroundWindow(handle);
            }
            catch (Exception)
            {
                return false;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
