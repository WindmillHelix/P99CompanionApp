using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Models.Maps;

namespace WindmillHelix.Companion99.Services.Maps
{
    public class MapService : IMapService
    {
        private readonly List<string> _ignoreLayers = new List<string>();

        public MapService()
        {
            _ignoreLayers.Add("2");
        }

        private string GetMapFolder()
        {
            return @"C:\Games\EQ\maps";
        }

        public Map GetMap(string zoneShortName)
        {
            var layers = new List<MapLayer>();
            var map = new Map();
            map.Layers = layers;

            var baseFileName = Path.Combine(GetMapFolder(), zoneShortName + ".txt");
            if (File.Exists(baseFileName))
            {
                var baseLayer = OpenLayer(baseFileName);
                baseLayer.Name = "Base";
                layers.Add(baseLayer);
            }

            var files = Directory.GetFiles(GetMapFolder(), zoneShortName + "_*.txt");

            foreach (var file in files.OrderBy(x => x))
            {
                var info = new FileInfo(file);

                var name = info.Name.Substring(zoneShortName.Length + 1, info.Name.Length - zoneShortName.Length - 5);
                if (!_ignoreLayers.Contains(name))
                {
                    var layer = OpenLayer(file);
                    layer.Name = name;
                    layers.Add(layer);
                }
            }

            return map;
        }

        private MapLayer OpenLayer(string fullFileName)
        {
            var layer = new MapLayer();
            var lines = new List<Line>();
            var points = new List<Models.Maps.Point>();

            layer.Points = points;
            layer.Lines = lines;

            var fileLines = File.ReadAllLines(fullFileName);
            foreach (var fileLine in fileLines)
            {
                if (fileLine.StartsWith("L "))
                {
                    var line = ParseLine(fileLine);
                    lines.Add(line);
                }
                if (fileLine.StartsWith("P "))
                {
                    var point = ParsePoint(fileLine);
                    points.Add(point);
                }
            }

            return layer;
        }

        private Line ParseLine(string text)
        {
            var parts = text.Substring(2).Split(',');
            var line = new Line
            {
                Point1 = CreatePoint(double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2])),
                Point2 = CreatePoint(double.Parse(parts[3]), double.Parse(parts[4]), double.Parse(parts[5])),
            };

            var color = Color.FromArgb(byte.Parse(parts[6]), byte.Parse(parts[7]), byte.Parse(parts[8]));
            line.Color = color;

            return line;
        }

        private Models.Maps.Point ParsePoint(string text)
        {
            var parts = text.Substring(2).Split(',');
            var point = CreatePoint(double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2]));
            point.Label = parts[7].Trim().Replace("_", " ");

            var color = Color.FromArgb(byte.Parse(EmptyValue(parts[3], "0")), byte.Parse(parts[4]), byte.Parse(parts[5]));
            point.Color = color;

            return point;
        }

        private string EmptyValue(string source, string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return defaultValue;
            }

            return source;
        }

        private Models.Maps.Point CreatePoint(double x, double y, double z)
        {
            return new Models.Maps.Point
            {
                X = x,
                Y = y,
                Z = z
            };
        }
    }
}
