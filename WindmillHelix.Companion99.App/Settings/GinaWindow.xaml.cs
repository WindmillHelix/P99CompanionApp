using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.Settings
{
    /// <summary>
    /// Interaction logic for GinaWindow.xaml
    /// </summary>
    public partial class GinaWindow : Window
    {
        private readonly IConfigurationService _configurationService;
        private readonly IGinaService _ginaService;

        private bool _isInitializing;

        public GinaWindow()
        {
            InitializeComponent();
            this.SetupDefaults();
            _configurationService = DependencyInjector.Resolve<IConfigurationService>();
            _ginaService = DependencyInjector.Resolve<IGinaService>();

            _isInitializing = true;
            AutoStartGinaCheckBox.IsChecked = _configurationService.ShouldAutoStartGina;
            RefreshGinaStatus();
            _isInitializing = false;
        }

        private void AutoStartGinaCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(_isInitializing)
            {
                return;
            }

            _configurationService.ShouldAutoStartGina = AutoStartGinaCheckBox.IsChecked ?? false;

            if (_configurationService.ShouldAutoStartGina)
            {
                _ginaService.EnsureGinaRunning();
            }
        }

        private void RefreshGinaStatus()
        {
            var isRunning = _ginaService.IsGinaRunning();
            var isDetected = _ginaService.IsGinaDetected();

            var status = string.Empty;
            if(isRunning)
            {
                status = "Running";
            }
            else if(isDetected)
            {
                status = "Not Running";
            }
            else
            {
                status = "Not Detected";
            }

            GinaStatusLabel.Content = status;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if(_configurationService.ShouldAutoStartGina)
            {
                _ginaService.EnsureGinaRunning();
            }
        }
    }
}
