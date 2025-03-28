using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Data
{
    public class KeyValueRepository : IKeyValueRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public KeyValueRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public IReadOnlyDictionary<string, string> GetAllValues()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM KeyValuePair";
                var reader = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                var items = dataTable.AsEnumerable().ToDictionary(x => x.Field<string>("ItemKey"), x => x.Field<string>("ItemValue"));
                return items;
            }
        }

        public void SetValue(string key, string value)
        {
            var sql = @"INSERT INTO KeyValuePair(ItemKey, ItemValue) VALUES($itemKey, $itemValue)
                ON CONFLICT(ItemKey) DO UPDATE SET ItemValue = excluded.ItemValue";

            using(var conn = _sqlConnectionFactory.CreateConnection())
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddWithValue("$itemKey", key);
                command.Parameters.AddWithValue("$itemValue", value);

                command.ExecuteNonQuery();
            }
        }
    }
}
