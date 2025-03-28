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
using WindmillHelix.Companion99.Services.Events;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for KeysWindow.xaml
    /// </summary>
    public partial class KeysWindow : Window
    {
        private const int VeeshansPeakKey = 1817;
        private const int SebilisKey = 1810;

        private bool _isInitialized = false;

        public KeysWindow()
        {
            InitializeComponent();
            this.SetupDefaults();
        }

        public IReadOnlyCollection<InventoryItem> Items { get; set; }

        public string CharacterName { get; set; }


        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            this.Title = $"P99 Companion - Keys - {CharacterName}";
            if (!_isInitialized)
            {
                var toDisplay = new int[] { SebilisKey, VeeshansPeakKey };
                foreach (var key in toDisplay)
                {
                    var child = new QuestItemDisplayControl();
                    child.ItemId = key;
                    child.CharacterName = CharacterName;
                    KeysPanel.Children.Add(child);
                }

                _isInitialized = true;
            }
        }
    }
}
