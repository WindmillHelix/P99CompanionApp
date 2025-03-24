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
using WindmillHelix.Companion99.Common.Threading;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.Settings
{
    /// <summary>
    /// Interaction logic for MiddlemanWindow.xaml
    /// </summary>
    public partial class MiddlemanWindow : Window
    {
        private readonly IMiddlemanService _middlemanService;
        private readonly IConfigurationService _configurationService;
        private bool _isUIPopulating = false;

        public MiddlemanWindow()
        {
            InitializeComponent();

            _middlemanService = DependencyInjector.Resolve<IMiddlemanService>();
            _configurationService = DependencyInjector.Resolve<IConfigurationService>();

            PopulateUI();
        }

        private void PopulateUI()
        {
            _isUIPopulating = true;
            try
            {
                var currentHost = _middlemanService.GetCurrentHostName();
                HostTextBox.Text = _middlemanService.GetCurrentHostName();

                var isUsingMiddleman = currentHost.Equals(MiddlemanConstants.MiddlemanHost, StringComparison.OrdinalIgnoreCase);

                AutoStartMiddlemanCheckBox.IsChecked = isUsingMiddleman && _configurationService.ShouldAutoStartMiddleman;
                AutoStartMiddlemanCheckBox.IsEnabled = isUsingMiddleman;

                var isRunning = _middlemanService.IsMiddlemanRunning();
                MiddlemanStatusLabel.Content = isRunning ? "Running" : "Not Running";

                StopMiddlemanButton.Visibility = isRunning ? Visibility.Visible : Visibility.Hidden;
            }
            finally
            {
                _isUIPopulating = false;
            }
        }

        private void UseMiddlemanButton_Click(object sender, RoutedEventArgs e)
        {
            AsyncHelper.RunSynchronously(() => UseMiddlemanAsync());
            PopulateUI();
        }

        private async Task UseMiddlemanAsync()
        {
            await _middlemanService.StartMiddlemanAsync();
            _middlemanService.SetHostName(MiddlemanConstants.MiddlemanHost);
            _configurationService.ShouldAutoStartMiddleman = true;
        }

        private void UseDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            _middlemanService.SetHostName(MiddlemanConstants.StandardHost);
            _configurationService.ShouldAutoStartMiddleman = false;

            PopulateUI();
        }

        private void AutoStartMiddlemanCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(_isUIPopulating)
            {
                return;
            }

            _configurationService.ShouldAutoStartMiddleman = AutoStartMiddlemanCheckBox.IsChecked ?? false;
        }

        private void StopMiddlemanButton_Click(object sender, RoutedEventArgs e)
        {
            _middlemanService.StopMiddleman();
            PopulateUI();
        }
    }
}
