using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Services
{
    public interface ILogService
    {
        public void LogException(
            Exception thrown,
            OptionalParameterDemarc optionalParameterDemarc = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "");

        public void Log(
            string message,
            OptionalParameterDemarc optionalParameterDemarc = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "");
    }
}
