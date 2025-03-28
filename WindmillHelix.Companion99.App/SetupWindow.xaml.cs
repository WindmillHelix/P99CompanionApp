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
using WindmillHelix.Companion99.App.Events;
using WindmillHelix.Companion99.Services;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        private readonly IConfigurationService _configurationService;
        private readonly IEventService _eventService;

        public SetupWindow()
        {
            InitializeComponent();
            this.SetupDefaults();

            _configurationService = DependencyInjector.Resolve<IConfigurationService>();
            _eventService = DependencyInjector.Resolve<IEventService>();

            EverQuestFolderTextBox.Text = _configurationService.EverQuestFolder;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var location = EverQuestFolderTextBox.Text;
            if (_configurationService.IsValidEverQuestFolder(location))
            {
                _configurationService.EverQuestFolder = location;
                _eventService.Raise<EverQuestFolderConfiguredEvent>(new EverQuestFolderConfiguredEvent());
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid EverQuest Directory");
            }
        }

        private void FolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = EverQuestFolderTextBox.Text;
            var result = dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                EverQuestFolderTextBox.Text = dialog.SelectedPath;
            }
        }
    }
}
