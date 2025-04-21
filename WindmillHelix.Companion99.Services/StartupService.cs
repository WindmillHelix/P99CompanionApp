using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Data;

namespace WindmillHelix.Companion99.Services
{
    public class StartupService : IStartupService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IDatabaseInitializer _databaseInitializer;

        public StartupService(IConfigurationService configurationService, IDatabaseInitializer databaseInitializer)
        {
            _configurationService = configurationService;
            _databaseInitializer = databaseInitializer;
        }

        public void EnsureDataDirectoryExists()
        {
            var dataFolder = FileHelper.GetDataFolder();
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
        }

        public void InitializeDatabase()
        {
            _databaseInitializer.Execute();
        }

        public bool IsEverQuestDirectoryValid()
        {
            string folderLocation = _configurationService.EverQuestFolder;
            var result = _configurationService.IsValidEverQuestFolder(folderLocation);

            return result;
        }
    }
}
