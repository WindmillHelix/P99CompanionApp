using DiscordOverlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindmillHelix.Companion99.App.DiscordOverlay;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.Settings
{
    /// <summary>
    /// Interaction logic for DiscordOverlayWindow.xaml
    /// </summary>
    public partial class DiscordOverlayWindow : Window
    {
        private readonly IConfigurationService _configurationService;

        public DiscordOverlayWindow()
        {
            InitializeComponent();
            this.SetupDefaults();

            _configurationService = DependencyInjector.Resolve<IConfigurationService>();
            if(_configurationService.IsDiscordOverlayEnabled)
            {
                DiscordOverlayBroker.SetResizeMode();
            }
        }

        private void EnableButton_Click(object sender, RoutedEventArgs e)
        {
            _configurationService.IsDiscordOverlayEnabled = true;
            DiscordOverlayBroker.Enable();
            Thread.Sleep(1000);
            DiscordOverlayBroker.SetResizeMode();
        }

        private void DisableButton_Click(object sender, RoutedEventArgs e)
        {
            _configurationService.IsDiscordOverlayEnabled = false;
            DiscordOverlayBroker.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if(_configurationService.IsDiscordOverlayEnabled)
            {
                DiscordOverlayBroker.Start();
                DiscordOverlayBroker.SetRunMode();
            }
        }
    }
}
