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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindmillHelix.Companion99.App.Settings;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        private void DiscordIntegrationButton_Click(object sender, RoutedEventArgs e)
        {
            var window = DependencyInjector.Resolve<DiscordConfigWindow>();
            window.ShowDialog();
        }

        private void MiddlemanButton_Click(object sender, RoutedEventArgs e)
        {
            var window = DependencyInjector.Resolve<MiddlemanWindow>();
            window.ShowDialog();
        }

        private void MapsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = DependencyInjector.Resolve<MapConfigWindow>();
            window.ShowDialog();
        }
    }
}
