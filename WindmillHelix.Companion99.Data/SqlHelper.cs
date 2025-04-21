using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Data
{
    internal static class SqlHelper
    {
        public static string GetSqliteFilePath()
        {
            var file = Path.Combine(FileHelper.GetDataFolder(), "settings.db");
            return file;
        }
    }
}
