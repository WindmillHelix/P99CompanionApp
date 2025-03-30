using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Data.Models;

namespace WindmillHelix.Companion99.Data.Migrators
{
    public class LastZoneMigrator : IMigrator
    {
        private readonly IParkInformationRepository _parkInformationRepository;

        public LastZoneMigrator(IParkInformationRepository parkInformationRepository)
        {
            _parkInformationRepository = parkInformationRepository;
        }

        public string Key => "LastZone";

        public void Execute()
        {
            var fileName = Path.Combine(FileHelper.GetDataFolder(), "LastZone.xml");
            var serializer = new XmlSerializer(typeof(CharacterZone[]));
            if (!File.Exists(fileName))
            {
                return;
            }

            using (var fs = new FileStream(fileName, FileMode.Open))
            {
                var items = (CharacterZone[])serializer.Deserialize(fs);
                foreach (var item in items)
                {
                    item.CharacterName = NamingUtil.FixCharacterCasing(item.CharacterName);
                    _parkInformationRepository.SaveParkInformation(item.ToParkInfo());
                }
            }
        }

        public class CharacterZone
        {
            public string ServerName { get; set; }

            public string CharacterName { get; set; }

            public string ZoneName { get; set; }

            public string Account { get; set; }

            public DateTime? SkyCorpseDate { get; set; }

            public string BindZone { get; set; }

            public ParkInformationModel ToParkInfo()
            {
                return new ParkInformationModel
                {
                    ServerName = ServerName,
                    CharacterName = CharacterName,
                    ZoneName = ZoneName,
                    Account = Account,
                    SkyCorpseDate = SkyCorpseDate,
                    BindZone = BindZone
                };
            }
        }
    }
}
