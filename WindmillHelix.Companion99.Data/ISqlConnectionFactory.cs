using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Data
{
    public interface ISqlConnectionFactory
    {
        SqliteConnection CreateConnection();
    }
}
