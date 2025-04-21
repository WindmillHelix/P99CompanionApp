using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common
{
    public static class NamingUtil
    {
        public static string FixCharacterCasing(string characterName)
        {
            var fixedName = characterName.Substring(0, 1).ToUpper() + characterName.Substring(1).ToLower();
            return fixedName;
        }
    }
}
