using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WindmillHelix.Companion99.App.Models;

namespace WindmillHelix.Companion99.App.ViewModels
{
    public class RollSetViewModel : INotifyPropertyChanged
    {
        private const int _expansionHeight = 130;

        private IReadOnlyCollection<RollViewModel> _sortedRolls = new List<RollViewModel>();

        private static Thickness _notExpandedMargin = new Thickness(0, 0, 0, 0);
        private static Thickness _expandedMargin = new Thickness(0, 0, 0, _expansionHeight);

        private bool _isExpanded = false;

        public RollSetViewModel()
        {
            Rolls = new ObservableCollection<RollViewModel>();
            Rolls.CollectionChanged += Rolls_CollectionChanged;
            LastRoll = DateTime.Now;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Rolls_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _sortedRolls = Rolls.OrderByDescending(x => x.Value).ThenBy(x => x.CharacterName).ToList();
            NotifyPropertyChanged(nameof(SortedRolls));
            NotifyPropertyChanged(nameof(HighestRoll));

            NotifyPropertyChanged(nameof(LeaderNames));

            LastRoll = DateTime.Now;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public DateTime LastRoll { get; private set; }

        public int ExpansionHeight
        {
            get { return _expansionHeight; }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                _isExpanded = value;
                NotifyPropertyChanged(nameof(IsExpanded));
                NotifyPropertyChanged(nameof(DetailsVisibility));
                NotifyPropertyChanged(nameof(ElementMargin));
                NotifyPropertyChanged(nameof(ExpandButtonContent));
            }
        }

        public string ExpandButtonContent
        {
            get
            {
                return IsExpanded ? "-" : "+";
            }
        }

        public Visibility DetailsVisibility
        {
            get
            {
                return IsExpanded ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Thickness ElementMargin
        {
            get
            {
                return IsExpanded ? _expandedMargin : _notExpandedMargin;
            }
        }

        public int Min { get; set; }

        public int Max { get; set; }

        public DateTime FirstRoll { get; set; }

        public ObservableCollection<RollViewModel> Rolls { get; private set; }

        public IReadOnlyCollection<RollViewModel> SortedRolls
        {
            get
            {
                return _sortedRolls;
            }
        }

        public int HighestRoll
        {
            get
            {
                return _sortedRolls.First().Value;
            }
        }

        public string LeaderNames
        {
            get
            {
                var max = this.HighestRoll;
                var matches = Rolls.Where(x => x.Value == max).OrderBy(x => x.CharacterName).ToList();

                var builder = new StringBuilder();
                bool isFirst = true;

                foreach(var match in matches)
                {
                    if(!isFirst)
                    {
                        builder.Append(", ");
                    }

                    builder.Append(match.CharacterName);
                    var rollCount = Rolls.Count(x => x.CharacterName == match.CharacterName); 

                    if(rollCount != 1)
                    {
                        builder.Append($" ({rollCount})");
                    }

                    isFirst = false;
                }

                var names = builder.ToString();
                return names;
            }
        }

        public string Range
        {
            get
            {
                return $"[{this.Min}-{this.Max}]";
            }
        }
    }
}
