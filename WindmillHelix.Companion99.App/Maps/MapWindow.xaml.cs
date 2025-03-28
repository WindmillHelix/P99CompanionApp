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
using WindmillHelix.Companion99.Services.Maps;
using WindmillHelix.Companion99.Services.Models.Maps;

namespace WindmillHelix.Companion99.App.Maps
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window, IEventSubscriber<CurrentLocation>
    {
        private readonly IMapService _mapService;
        private readonly IZoneLookupService _zoneLookupService;

        private readonly ICurrentLocationService _currentLocationService;

        private string _currentZoneShortName;

        private Map _map;
        private List<MapElement> _elements = new List<MapElement>();
        private Dictionary<Companion99.Services.Models.Maps.Point, System.Windows.Shapes.Line> _points
            = new Dictionary<Companion99.Services.Models.Maps.Point, System.Windows.Shapes.Line>();

        private System.Windows.Shapes.Line _playerMarker;
        private bool _isPlayerMarkerVisible = false;

        private double _minX;
        private double _minY;
        private double _maxX;
        private double _maxY;

        public MapWindow()
        {
            InitializeComponent();
            this.SetupDefaults();

            _mapService = DependencyInjector.Resolve<IMapService>();
            _zoneLookupService = DependencyInjector.Resolve<IZoneLookupService>();
            var eventService = DependencyInjector.Resolve<IEventService>();
            eventService.AddSubscriber<CurrentLocation>(this);

            _currentLocationService = DependencyInjector.Resolve<ICurrentLocationService>();
            SetUpPlayerMarker();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if(_currentZoneShortName == null)
            {
                var currentLocation = _currentLocationService.CurrentLocation;
                if(!string.IsNullOrEmpty(currentLocation?.ZoneShortName))
                {
                    TryLoadZone(currentLocation.ZoneShortName);
                    if(currentLocation.Location != null)
                    {
                        SetLocation(currentLocation.Location);
                    }
                }
            }
        }

        private void SetUpPlayerMarker()
        {
            _playerMarker = new System.Windows.Shapes.Line();
            var thickness = new Thickness(8);
            _playerMarker.Margin = thickness;
            _playerMarker.Visibility = System.Windows.Visibility.Visible;
            _playerMarker.StrokeThickness = 8;
            _playerMarker.Stroke = System.Windows.Media.Brushes.Yellow;
        }

        private void LoadMap(Map map)
        {
            _map = map;
            _elements.Clear();
            this.Dispatcher.Invoke(() => DrawMap());
        }

        private void DrawMap()
        {
            MainCanvas.Children.Clear();
            _points.Clear();

            _isPlayerMarkerVisible = false;

            if (_map == null)
            {
                return;
            }

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;

            foreach (var layer in _map.Layers.Where(x => (x.Lines?.Count ?? 0) > 0))
            {
                var minX1 = layer.Lines.Select(x => x.Point1.X).Min();
                var minX2 = layer.Lines.Select(x => x.Point2.X).Min();
                var maxX1 = layer.Lines.Select(x => x.Point1.X).Max();
                var maxX2 = layer.Lines.Select(x => x.Point2.X).Max();

                var minY1 = layer.Lines.Select(x => x.Point1.Y).Min();
                var minY2 = layer.Lines.Select(x => x.Point2.Y).Min();
                var maxY1 = layer.Lines.Select(x => x.Point1.Y).Max();
                var maxY2 = layer.Lines.Select(x => x.Point2.Y).Max();

                minX = Min(minX1, minX2, minX);
                minY = Min(minY1, minY2, minY);

                maxX = Max(maxX1, maxX2, maxX);
                maxY = Max(maxY1, maxY2, maxY);
            }

            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;

            var width = Math.Ceiling(Math.Abs(maxX - minX));
            var height = Math.Ceiling(Math.Abs(maxY - minY));

            var maxDimension = Math.Max(width, height);
            var thicknessInPixels = Math.Floor(maxDimension / 1000);
            if (thicknessInPixels < 2)
            {
                thicknessInPixels = 2;
            }

            var thickness = new Thickness(thicknessInPixels);

            _playerMarker.StrokeThickness = thicknessInPixels * 6;

            MainCanvas.Width = width;
            MainCanvas.Height = height;

            int layerNumber = 0;
            var orderedLayers = _map.Layers.OrderBy(x => x.Name != "Base").ThenBy(x => x.Name).ToList();
            foreach (var layer in orderedLayers)
            {
                foreach (var line in layer.Lines)
                {
                    var toDraw = new System.Windows.Shapes.Line();
                    toDraw.Margin = thickness;
                    toDraw.Visibility = System.Windows.Visibility.Visible;
                    toDraw.StrokeThickness = thicknessInPixels;
                    toDraw.Stroke = new SolidColorBrush(Color.FromArgb(line.Color.A, line.Color.R, line.Color.G, line.Color.B));
                    toDraw.Stroke = System.Windows.Media.Brushes.White;
                    toDraw.X1 = line.Point1.X - minX;
                    toDraw.Y1 = line.Point1.Y - minY;
                    toDraw.X2 = line.Point2.X - minX;
                    toDraw.Y2 = line.Point2.Y - minY;

                    toDraw.Tag = line;

                    Canvas.SetZIndex(toDraw, layerNumber * 10);
                    MainCanvas.Children.Add(toDraw);

                    var mapElement = new MapElement
                    {
                        Elements = new[] { toDraw },
                        MapElementType = MapElementType.Line,
                        LayerName = layer.Name
                    };

                    _elements.Add(mapElement);
                }

                var offsetX = (maxX + minX) / 2;
                var offsetY = (maxY + minY) / 2;

                offsetX = 0;
                offsetY = 0;

                foreach (var point in layer.Points)
                {
                    var dot = new System.Windows.Shapes.Line();
                    dot.Visibility = System.Windows.Visibility.Visible;
                    dot.StrokeThickness = thicknessInPixels * 4;
                    dot.Stroke = System.Windows.Media.Brushes.Orange;
                    dot.X1 = point.X - minX + offsetX;
                    dot.Y1 = point.Y - minY + offsetY;
                    dot.X2 = point.X - minX + offsetX + 4 * thicknessInPixels;
                    dot.Y2 = point.Y - minY + offsetY;

                    dot.Tag = point;
                    var tooltip = new ToolTip();
                    tooltip.Content = point.Label;
                    dot.ToolTip = tooltip;

                    _points.Add(point, dot);

                    Canvas.SetZIndex(dot, layerNumber * 10 + 1);
                    MainCanvas.Children.Add(dot);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = point.Label;
                    textBlock.FontSize = thicknessInPixels * 4 * 2;
                    textBlock.Foreground = new SolidColorBrush(Colors.Orange);
                    Canvas.SetLeft(textBlock, dot.X2 + 3);
                    Canvas.SetTop(textBlock, dot.Y1 - thicknessInPixels * 4);

                    Canvas.SetZIndex(textBlock, layerNumber * 10 + 1);
                    MainCanvas.Children.Add(textBlock);

                    var mapElement = new MapElement
                    {
                        Elements = new FrameworkElement[] { dot, textBlock },
                        MapElementType = MapElementType.Line,
                        LayerName = layer.Name,
                        FilterText = point.Label
                    };

                    _elements.Add(mapElement);
                }

                layerNumber++;
            }

            ApplyFilters();
        }

        private double Min(params double[] values)
        {
            return values.Min();
        }

        private double Max(params double[] values)
        {
            return values.Max();
        }

        private void TryLoadZone(string zoneName)
        {
            if (!string.IsNullOrEmpty(zoneName))
            {
                var map = _mapService.GetMap(zoneName);
                if (map != null)
                {
                    LoadMap(map);
                }
            }
            else
            {
                LoadMap(null);
            }
        }

        private void SetLocation(Location location)
        {
            // todo: use last location to determine arrow
            this.Dispatcher.Invoke(() =>
            {
                var offsetX = -132;
                var offsetY = 30;

                offsetX = 0;
                offsetY = 0;

                _playerMarker.X1 = (-1 * location.X) - _minX + offsetX;
                _playerMarker.Y1 = (-1 * location.Y) - _minY + offsetY;
                _playerMarker.X2 = (-1 * location.X) - _minX + offsetX + _playerMarker.StrokeThickness;
                _playerMarker.Y2 = (-1 * location.Y) - _minY + offsetY;

                if (!_isPlayerMarkerVisible)
                {
                    MainCanvas.Children.Add(_playerMarker);
                    _isPlayerMarkerVisible = true;
                }
            });
        }

        public Task Handle(CurrentLocation value)
        {
            if(value.ZoneShortName != _currentZoneShortName)
            {
                _currentZoneShortName = value.ZoneShortName;
                TryLoadZone(value.ZoneShortName);
            }
            else if(value.Location != null)
            {
                SetLocation(value.Location);
            }

            return Task.CompletedTask;
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var value = FilterTextBox.Text;
            foreach (var item in _elements)
            {
                if (!string.IsNullOrEmpty(item.FilterText))
                {
                    bool shouldBeVisible = false;

                    if (string.IsNullOrEmpty(value) || item.FilterText.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        shouldBeVisible = true;
                    }

                    foreach (var element in item.Elements)
                    {
                        element.Visibility = shouldBeVisible ? Visibility.Visible : Visibility.Hidden;
                    }
                }
            }
        }
    }
}
