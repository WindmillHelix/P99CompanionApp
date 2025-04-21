using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Data
{
    public static partial class ExtensionMethods
    {
        public static void AddX(this SqliteParameterCollection parameters, string parameterName, object value)
        {
            parameters.AddWithValue(parameterName, value == null ? DBNull.Value : value);
        }
    }
}
