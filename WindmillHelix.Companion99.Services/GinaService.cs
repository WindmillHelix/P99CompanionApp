using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common.Interop;

namespace WindmillHelix.Companion99.Services
{
    public class GinaService : IGinaService
    {
        private const string ProcessName = "GINA";

        public void EnsureGinaRunning()
        {
            if(IsGinaRunning())
            {
                return;
            }

            if(!IsGinaDetected())
            {
                return;
            }

            var shortcutPath = GetShortcutPath();

            var start = new ProcessStartInfo(shortcutPath);
            start.UseShellExecute = true;

            var handle = Process.GetCurrentProcess().MainWindowHandle; ;
            User32Interop.ShowWindow(handle, 5);
            User32Interop.SetForegroundWindow(handle);

            var process = Process.Start(start);
        }

        private string GetShortcutPath()
        {
            var shortcutPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                "GimaSoft",
                "GINA.appref-ms");

            return shortcutPath;
        }

        public bool IsGinaRunning()
        {
            var processes = Process.GetProcessesByName(ProcessName);
            var result = processes.Length > 0;
            return result;
        }

        public bool IsGinaDetected()
        {
            var shortcutPath = GetShortcutPath();
            var result = File.Exists(shortcutPath);
            return result;
        }
    }
}
