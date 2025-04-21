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
using WindmillHelix.Companion99.Services.Models;
using WindmillHelix.Companion99.Common;
using System.Collections.ObjectModel;

namespace WindmillHelix.Companion99.App
{
    /// <summary>
    /// Interaction logic for EncounterLogsWindow.xaml
    /// </summary>
    public partial class EncounterLogsWindow : Window
    {
        private IReadOnlyCollection<WhoResult> _logs = new List<WhoResult>();

        public ObservableCollection<string> AssembledLogs { get; private set; }

        public EncounterLogsWindow()
        {
            InitializeComponent();
            this.DataContext = this;

            AssembledLogs = new ObservableCollection<string>();
        }

        public IReadOnlyCollection<WhoResult> Logs
        {
            get
            {
                return _logs;
            }

            set
            {
                _logs = value;
                SetupControls();
            }
        }

        private void SetupControls()
        {
            AssembledLogs.Clear();
            var reassembled = _logs
                .OrderBy(x => x.Name)
                .Select(x => $"[{x.EventDate.FormatForEverquestLog()}] {x.OriginalLine}")
                .ToList();

            const int maxLines = 50;
            const int maxSize = 2000;

            int lineCount = 0;
            string assembled = string.Empty;

            foreach(var line in reassembled)
            {
                if(lineCount + 1 > maxLines || (assembled.Length + line.Length + 2) > maxSize)
                {
                    AssembledLogs.Add(assembled);
                    lineCount = 0;
                    assembled = string.Empty;
                }

                if(lineCount != 0)
                {
                    assembled += "\r\n";
                }

                lineCount++;
                assembled += line;
            }

            if(lineCount > 0)
            {
                AssembledLogs.Add(assembled);
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Clipboard.SetText(button.Tag as string);
            button.Content = "✓ Copy";
        }
    }
}
