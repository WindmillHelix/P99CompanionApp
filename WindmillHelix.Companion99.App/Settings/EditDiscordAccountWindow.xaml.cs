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
using WindmillHelix.Companion99.Services.Discord;

namespace WindmillHelix.Companion99.App.Settings
{
    /// <summary>
    /// Interaction logic for EditDiscordAccountWindow.xaml
    /// </summary>
    public partial class EditDiscordAccountWindow : Window
    {
        private readonly IDiscordConfigService _configService;
        private Guid _discordAccountId;

        public EditDiscordAccountWindow()
        {
            _configService = DependencyInjector.Resolve<IDiscordConfigService>();
            InitializeComponent();
            this.SetupDefaults();
        }

        public void SetAccount(DiscordAccount account)
        {
            _discordAccountId = account.DiscordAccountId;
            NameTextBox.Text = account.Name;
            TokenTextBox.Text = account.AccountToken;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var account = new DiscordAccount
            {
                DiscordAccountId = _discordAccountId,
                Name = NameTextBox.Text,
                AccountToken = TokenTextBox.Text
            };

            var saveResult = _configService.SaveAccount(account);
            if(!saveResult.WasSuccessful)
            {
                ErrorLabel.Content = saveResult.FailureReason;   
            }
            else
            {
                this.Close();
            }
        }
    }
}
