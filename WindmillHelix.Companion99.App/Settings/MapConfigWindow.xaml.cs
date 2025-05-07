using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App.Settings
{
    /// <summary>
    /// Interaction logic for MapConfigWindow.xaml
    /// </summary>
    public partial class MapConfigWindow : Window
    {
        private readonly IConfigurationService _configurationService;

        public MapConfigWindow()
        {
            InitializeComponent();
            this.SetupDefaults();

            _configurationService = DependencyInjector.Resolve<IConfigurationService>();

            MapFolderTextBox.Text = _configurationService.MapsFolder;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _configurationService.MapsFolder = MapFolderTextBox.Text;
            this.Close();
        }

        private void MapFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = MapFolderTextBox.Text;

            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                MapFolderTextBox.Text = dialog.SelectedPath;
            }
        }
    }
}
