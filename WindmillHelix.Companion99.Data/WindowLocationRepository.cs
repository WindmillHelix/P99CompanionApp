using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Data.Models;

namespace WindmillHelix.Companion99.Data
{
    public class WindowLocationRepository : IWindowLocationRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public WindowLocationRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public WindowLocationModel GetByWindowName(string windowName)
        {
            var sql = @"SELECT * FROM WindowLocation WHERE WindowName = $windowName";


            using (var connection = _connectionFactory.CreateConnection())
            {

                var parameter = new SqliteParameter("$windowName", windowName);
                var rows = connection.GetResults(sql, parameter);

                var items = rows.Select(ConvertRowToModel).ToList();
                return items.SingleOrDefault();
            }
        }

        public void SaveWindowLocation(WindowLocationModel model)
        {
            var sql = @"INSERT INTO WindowLocation
                (WindowName, Size, Location)
                VALUES($windowName, $size, $location)
                ON CONFLICT(WindowName) DO UPDATE SET 
                    Size = excluded.Size,
                    Location = excluded.Location
                ";

            using (var conn = _connectionFactory.CreateConnection())
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddX("$windowName", model.WindowName);
                command.Parameters.AddX("$size", SerializeSize(model.Size));
                command.Parameters.AddX("$location", SerializePoint(model.Location));

                command.ExecuteNonQuery();
            }
        }

        private string SerializeSize(Size? value)
        {
            if(!value.HasValue)
            {
                return null;
            }

            return $"{value.Value.Width},{value.Value.Height}";
        }

        private string SerializePoint(Point value)
        {
            return $"{value.X},{value.Y}";
        }

        private Point DeserializePoint(string value)
        {
            var items = value.Split(',');
            var x = int.Parse(items[0]);
            var y = int.Parse(items[1]);

            return new Point(x, y);
        }

        private Size? DeserializeSize(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var point = DeserializePoint(value);
            return new Size(point);
        }

        private WindowLocationModel ConvertRowToModel(DataRow r)
        {
            var result = new WindowLocationModel()
            {
                WindowName = r.Field<string>(nameof(WindowLocationModel.WindowName)),
                Location = DeserializePoint(r.Field<string>(nameof(WindowLocationModel.Location))),
                Size = DeserializeSize(r.Field<string>(nameof(WindowLocationModel.Size)))
            };

            return result;
        }
    }
}

