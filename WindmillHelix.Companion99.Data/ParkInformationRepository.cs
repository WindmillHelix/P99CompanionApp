using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Data.Models;

namespace WindmillHelix.Companion99.Data
{
    public class ParkInformationRepository : IParkInformationRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public ParkInformationRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IReadOnlyCollection<ParkInformationModel> GetAll()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var rows = connection.GetResults("SELECT * FROM ParkInformation");

                var items = rows.Select(ConvertRowToModel).ToList();
                return items;
            }
        }

        public void SaveParkInformation(ParkInformationModel model)
        {
            var sql = @"INSERT INTO ParkInformation
                (ServerName, CharacterName, Account, ZoneName, BindZone, SkyCorpseDate, IsIgnored) 
                VALUES($serverName, $characterName, $account, $zoneName, $bindZone, $skyCorpseDate, $isIgnored)
                ON CONFLICT(ServerName, CharacterName) DO UPDATE SET 
                    Account = excluded.Account,
                    ZoneName = excluded.ZoneName,
                    BindZone = excluded.BindZone,
                    SkyCorpseDate = excluded.SkyCorpseDate,
                    IsIgnored = excluded.IsIgnored
                ";

            using (var conn = _connectionFactory.CreateConnection())
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddX("$serverName", model.ServerName);
                command.Parameters.AddX("$characterName", model.CharacterName);
                command.Parameters.AddX("$account", model.Account);
                command.Parameters.AddX("$zoneName", model.ZoneName);
                command.Parameters.AddX("$bindZone", model.BindZone);
                command.Parameters.AddX("$skyCorpseDate", DateUtil.ConvertToEpoch(model.SkyCorpseDate));
                command.Parameters.AddX("$isIgnored", model.IsIgnored ? 1 : 0);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteParkInformation(string serverName, string characterName)
        {
            var sql = @"DELETE FROM ParkInformation
                WHERE ServerName = $serverName AND CharacterName = $characterName";

            using (var conn = _connectionFactory.CreateConnection())
            {
                var command = conn.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddWithValue("$serverName", serverName);
                command.Parameters.AddWithValue("$characterName", characterName);

                command.ExecuteNonQuery();
            }
        }

        private ParkInformationModel ConvertRowToModel(DataRow r)
        {
            var result = new ParkInformationModel()
            {
                ServerName = r.Field<string>(nameof(ParkInformationModel.ServerName)),
                CharacterName = r.Field<string>(nameof(ParkInformationModel.CharacterName)),
                Account = r.Field<string>(nameof(ParkInformationModel.Account)),
                ZoneName = r.Field<string>(nameof(ParkInformationModel.ZoneName)),
                BindZone = r.Field<string>(nameof(ParkInformationModel.BindZone)),
                SkyCorpseDate = ConvertEpochDate(r.Field<long?>(nameof(ParkInformationModel.SkyCorpseDate))),
                IsIgnored = r.Field<long>(nameof(ParkInformationModel.IsIgnored)) == 1
            };

            return result;
        }

        private DateTime? ConvertEpochDate(long? source)
        {
            if (source == null)
            {
                return null;
            }

            var result = DateTime.UnixEpoch.AddSeconds(source.Value); ;
            return result;
        }
    }
}
