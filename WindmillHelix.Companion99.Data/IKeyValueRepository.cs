using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Data
{
    public interface IKeyValueRepository
    {
        void SetValue(string key, string value);

        IReadOnlyDictionary<string, string> GetAllValues();
    }
}
