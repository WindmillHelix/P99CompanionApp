using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Common
{
    public static class DateUtil
    {
        public static long? ConvertToEpoch(DateTime? value)
        {
            if (!value.HasValue)
            {
                return null;
            }

            var timespan = value.Value - DateTime.UnixEpoch;
            return (long)timespan.TotalSeconds;
        }
    }
}
