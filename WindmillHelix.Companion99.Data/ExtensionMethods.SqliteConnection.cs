using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Data
{
    public static partial class ExtensionMethods
    {
        public static EnumerableRowCollection<DataRow> GetResults(
            this SqliteConnection connection, 
            string sql, 
            params SqliteParameter[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddRange(parameters);
            var reader = command.ExecuteReader();
            var dataTable = new DataTable();
            dataTable.Load(reader);
            var rows = dataTable.AsEnumerable();

            return rows;
        }
    }
}
