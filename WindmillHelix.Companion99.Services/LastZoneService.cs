using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Data;
using WindmillHelix.Companion99.Data.Models;
using WindmillHelix.Companion99.Services.Models;

namespace WindmillHelix.Companion99.Services
{
    public class LastZoneService : ILastZoneService
    {
        private List<CharacterZone> _items;
        private bool _isInitialized = false;
        private object _lock = new object();
        private readonly IParkInformationRepository _parkInformationRepository;

        public LastZoneService(IParkInformationRepository parkInformationRepository)
        {
            _items = new List<CharacterZone>();
            _parkInformationRepository = parkInformationRepository;
        }

        public IReadOnlyCollection<CharacterZone> GetLastZones()
        {
            EnsureInitialized();
            return _items;
        }

        public void SetLastZone(string serverName, string characterName, string zoneName, string account)
        {
            UpdateEntry(
                serverName,
                characterName,
                a =>
                {
                    a.ZoneName = zoneName;
                    a.Account = account.ToLowerInvariant();
                });
        }

        public void SetSkyCorpseDate(string serverName, string characterName, DateTime dateOfDeath)
        {
            UpdateEntry(
                serverName,
                characterName,
                a =>
                {
                    a.SkyCorpseDate = dateOfDeath;
                });
        }

        public void SetBindPoint(string serverName, string characterName, string bindZone)
        {
            UpdateEntry(
                serverName,
                characterName,
                a =>
                {
                    a.BindZone = bindZone;
                });
        }

        public void RemoveEntry(string serverName, string characterName)
        {
            var item = _items.SingleOrDefault(
                x => x.ServerName.EqualsIngoreCase(serverName)
                && x.CharacterName.EqualsIngoreCase(characterName));

            if (item != null)
            {
                _items.Remove(item);
                _parkInformationRepository.DeleteParkInformation(item.ServerName, item.CharacterName);
            }
        }

        private ParkInformationModel ConvertToDatabaseModel(CharacterZone source)
        {
            var model = new ParkInformationModel()
            {
                Account = source.Account,
                BindZone = source.BindZone,
                CharacterName = source.CharacterName,
                ServerName = source.ServerName,
                SkyCorpseDate = source.SkyCorpseDate,
                ZoneName = source.ZoneName
            };

            return model;
        }

        private void UpdateEntry(string serverName, string characterName, Action<CharacterZone> action)
        {
            var item = _items.SingleOrDefault(
                x => x.ServerName.EqualsIngoreCase(serverName)
                && x.CharacterName.EqualsIngoreCase(characterName));

            if (item == null)
            {
                item = new CharacterZone
                {
                    ServerName = serverName,
                    CharacterName = NamingUtil.FixCharacterCasing(characterName)
                };

                _items.Add(item);
            }

            action(item);

            var model = ConvertToDatabaseModel(item);
            _parkInformationRepository.SaveParkInformation(model);
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (_lock)
                {
                    if (!_isInitialized)
                    {
                        var parkInfo = _parkInformationRepository.GetAll();
                        var items = parkInfo.Select(x => new CharacterZone
                        {
                            Account = x.Account,
                            CharacterName = x.CharacterName,
                            BindZone = x.BindZone,
                            ServerName = x.ServerName,
                            SkyCorpseDate = x.SkyCorpseDate,
                            ZoneName = x.ZoneName
                        }).ToList();

                        _items = items;
                        _isInitialized = true;
                    }
                }
            }
        }
    }
}
