using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services
{
    public interface IConfigurationService
    {
        string EverQuestFolder { get; set; }

        bool IsAncientCyclopsTimerEnabled { get; set; }

        bool ShouldAutoStartMiddleman { get; set; }

        bool IsDiscordOverlayEnabled { get; set; }

        Point DiscordOverlayLocation { get; set; }

        Point DiscordOverlaySize { get; set; }

        string MapsFolder { get; set; }

        bool IsValidEverQuestFolder(string folderLocation);
    }
}
