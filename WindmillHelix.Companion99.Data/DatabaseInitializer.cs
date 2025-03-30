using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Data.Migrators;

namespace WindmillHelix.Companion99.Data
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly IEnumerable<IMigrator> _migrators;
        private readonly IKeyValueRepository _keyValueRepository;

        public DatabaseInitializer(
            ISqlConnectionFactory connectionFactory, 
            IEnumerable<IMigrator> migrators, 
            IKeyValueRepository keyValueRepository)
        {
            _connectionFactory = connectionFactory;
            _migrators = migrators;
            _keyValueRepository = keyValueRepository;
        }

        public void Execute()
        {
            EnsureDatabaseExists();
            RunScript("SqlVersionTable.sql");
            RunSchemaScripts();
            RunMigrators();
        }

        private void EnsureDatabaseExists()
        {
            var fileName = SqlHelper.GetSqliteFilePath();
            if(!File.Exists(fileName))
            {
                var file = File.Create(fileName);
                file.Close();
            }
        }

        private void RunScript(string scriptName)
        {
            var sql = ResourceHelper.GetResourceString($"SqlScripts.{scriptName}");
            using(var conn = _connectionFactory.CreateConnection())
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                var result = command.ExecuteNonQuery();
            }
        }

        private void RunSchemaScripts()
        {
            var sql = "SELECT * FROM SqlVersion";
            List<string> executedScripts = new List<string>();
            using(var connection = _connectionFactory.CreateConnection())
            {
                var command = connection.CreateCommand();
                command.CommandText = sql;
                var reader = command.ExecuteReader();
                var dataTable = new DataTable();
                dataTable.Load(reader);
                executedScripts = dataTable.AsEnumerable().Select(x => x["ScriptName"].ToString()).ToList();
            }

            RunSchemaScript("001_InitialSchema.sql", executedScripts);
        }

        private void RunSchemaScript(string scriptName, List<string> alreadyExecuted)
        {
            if(alreadyExecuted.Contains(scriptName))
            {
                return;
            }

            RunScript(scriptName);
            alreadyExecuted.Add(scriptName);

            using (var conn = _connectionFactory.CreateConnection())
            {
                var command = conn.CreateCommand();
                command.CommandText = "INSERT INTO SqlVersion (ScriptName) VALUES ($scriptName)";
                command.Parameters.AddWithValue("$scriptName", scriptName);
                var result = command.ExecuteNonQuery();
            }
        }

        private void RunMigrators()
        {
            var keyValuePairs = _keyValueRepository.GetAllValues();
            foreach(var migrator in _migrators)
            {
                var key = $"MigrationComplete_{migrator.Key}";
                if(!keyValuePairs.ContainsKey(key))
                {
                    migrator.Execute();
                    _keyValueRepository.SetValue(key, "X");
                }
            }
        }
    }
}
