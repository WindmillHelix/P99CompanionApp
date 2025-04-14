using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common
{
    public static partial class ExtensionMethods
    {
        public static string FormatForEverquestLog(this DateTime value)
        {
            return DateUtil.FormatForEverquestLog(value);
        }
    }
}
