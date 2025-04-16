using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindmillHelix.Companion99.App.DiscordOverlay;
using WindmillHelix.Companion99.App.Maps;
using WindmillHelix.Companion99.App.Services;
using WindmillHelix.Companion99.Common.Threading;
using WindmillHelix.Companion99.Services;
using WindmillHelix.Companion99.Services.Discord;
using WindmillHelix.Companion99.Services.Events;
using WindmillHelix.Companion99.Services.Maps;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogReaderService _logReaderService;
        private readonly IConfigurationService _configurationService;
        private readonly IInventoryService _inventoryService;
        private readonly FileSystemWatcher _watcher;
        private readonly IEventService _eventService;
        private readonly IKillControlService _killControlService;

        private readonly IAppLaunchService _appLaunchService;

        // must keep a reference to these around
        private readonly IDiscordWorkerService _discordWorkerService;
        private readonly ICurrentLocationService _currentLocationService;

        public MainWindow()
        {
            InitializeComponent();
            this.SetupDefaults();

            _logReaderService = DependencyInjector.Resolve<ILogReaderService>();
            _configurationService = DependencyInjector.Resolve<IConfigurationService>();
            _inventoryService = DependencyInjector.Resolve<IInventoryService>();
            _eventService = DependencyInjector.Resolve<IEventService>();
            _killControlService = DependencyInjector.Resolve<IKillControlService>();
            _discordWorkerService = DependencyInjector.Resolve<IDiscordWorkerService>();
            _currentLocationService = DependencyInjector.Resolve<ICurrentLocationService>();

            _appLaunchService = DependencyInjector.Resolve<IAppLaunchService>();

            // purposely not awaited
            _appLaunchService.OnLaunchAsync();

            // purposely not awaited
            _discordWorkerService.StartAsync();

            _watcher = _inventoryService.CreateInventoryChangedWatcher();
            _watcher.Changed += HandleInventoryFilesChanged;
            _watcher.Created += HandleInventoryFilesChanged;
            _watcher.EnableRaisingEvents = true;

            AncientCyclopsTimerControl.Visibility = _configurationService.IsAncientCyclopsTimerEnabled ? Visibility.Visible : Visibility.Hidden;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            _logReaderService.Start();

            // purposely not awaited
            _appLaunchService.OnActivateAsync();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void HandleInventoryFilesChanged(object sender, FileSystemEventArgs e)
        {
            _eventService.Raise<InventoryFilesChangedEvent>();
        }

        private void StopwatchButton_Click(object sender, RoutedEventArgs e)
        {
            var stopwatchWindow = SingleWindowManager.GetWindow<StopwatchWindow>();
            stopwatchWindow.ShowOrActivate();
        }

        private void MapButton_Click(object sender, RoutedEventArgs e)
        {
            var mapWindow = SingleWindowManager.GetWindow<MapWindow>();
            mapWindow.ShowOrActivate();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            DiscordOverlayBroker.Stop();
        }
    }
}
