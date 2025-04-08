using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.App.DiscordOverlay;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.Services
{
    public class AppLaunchService : IAppLaunchService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IMiddlemanService _middlemanService;
        private readonly IGinaService _ginaService;

        public AppLaunchService(
            IConfigurationService configurationService, 
            IMiddlemanService middlemanService,
            IGinaService ginaService)
        {
            _configurationService = configurationService;
            _middlemanService = middlemanService;
            _ginaService = ginaService;
        }

        public Task OnLaunchAsync()
        {
            var task = Task.WhenAll(
                LaunchGinaAsync(),
                LaunchMiddlemanAsync());

            return task;
        }

        public Task OnActivateAsync()
        {
            var task = Task.WhenAll(
                StartDiscordOverlay());

            return task;
        }

        private Task LaunchGinaAsync()
        {
            if(_configurationService.ShouldAutoStartGina)
            {
                _ginaService.EnsureGinaRunning();
            }

            return Task.CompletedTask;
        }

        private async Task LaunchMiddlemanAsync()
        {
            if (_configurationService.ShouldAutoStartMiddleman)
            {
                await _middlemanService.StartMiddlemanAsync();
            }
        }

        private async Task StartDiscordOverlay()
        {
            if (_configurationService.IsDiscordOverlayEnabled)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                DiscordOverlayBroker.Start();
            }
        }
    }
}
