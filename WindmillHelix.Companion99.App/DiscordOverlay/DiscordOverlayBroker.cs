using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.App.DiscordOverlay
{
    public static class DiscordOverlayBroker
    {
        private static DiscordOverlayManager _discordOverlayManager = new DiscordOverlayManager();

        public static void Enable()
        {
            _discordOverlayManager.Enable();
        }

        public static void Start()
        {
            _discordOverlayManager.Start();
        }

        public static void SetResizeMode()
        {
            _discordOverlayManager.SetResizeMode();
        }

        public static void SetRunMode()
        {
            _discordOverlayManager.SetRunMode();
        }

        public static void Close()
        {
            _discordOverlayManager.Close();
        }
    }
}
