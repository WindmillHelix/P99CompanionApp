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

        public static void Start(Mode mode = Mode.Run)
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.Start(mode);
        }

        public static void SetResizeMode()
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.SetMode(Mode.Resize);
        }

        public static void SetRunMode()
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.SetMode(Mode.Run);
        }

        public static void Stop()
        {
            _logService.Value.Log("Invoked");
            _discordOverlayManager.Value.Stop();
        }
    }
}