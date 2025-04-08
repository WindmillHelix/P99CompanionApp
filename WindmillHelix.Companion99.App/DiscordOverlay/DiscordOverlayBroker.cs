using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.App.DiscordOverlay
{
    public static class DiscordOverlayBroker
    {
        private static Lazy<DiscordOverlayManager> _discordOverlayManager
            = new Lazy<DiscordOverlayManager>(() => DependencyInjector.Resolve<DiscordOverlayManager>());

        public static void Enable()
        {
            _discordOverlayManager.Value.Enable();
        }

        public static void Start()
        {
            _discordOverlayManager.Value.Start();
        }

        public static void SetResizeMode()
        {
            _discordOverlayManager.Value.SetResizeMode();
        }

        public static void SetRunMode()
        {
            _discordOverlayManager.Value.SetRunMode();
        }

        public static void Close()
        {
            _discordOverlayManager.Value.Close();
        }
    }
}