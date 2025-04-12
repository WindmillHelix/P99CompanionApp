using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.DiscordOverlay
{
    public static class DiscordOverlayBroker
    {
        private static Lazy<DiscordOverlayManager> _discordOverlayManager
            = new Lazy<DiscordOverlayManager>(() => DependencyInjector.Resolve<DiscordOverlayManager>());

        private static Lazy<ILogService> _logService
            = new Lazy<ILogService>(() => DependencyInjector.Resolve<ILogService>());

        public static void Enable()
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.Enable(Mode.Resize);
        }

        public static void Start()
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.Start();
        }

        public static void SetResizeMode()
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.SetResizeMode();
        }

        public static void SetRunMode()
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.SetRunMode();
        }

        public static void Close()
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.Close();
        }
    }
}