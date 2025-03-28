using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services
{
    public interface IStartupService
    {
        void EnsureDataDirectoryExists();

        void InitializeDatabase();

        bool IsEverQuestDirectoryValid();
    }
}
