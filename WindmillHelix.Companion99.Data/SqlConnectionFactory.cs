using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Data
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        public SqliteConnection CreateConnection()
        {
            var file = SqlHelper.GetSqliteFilePath();
            var connection = new SqliteConnection($"Data Source={file}");
            connection.Open();
            return connection;
        }
    }
}
