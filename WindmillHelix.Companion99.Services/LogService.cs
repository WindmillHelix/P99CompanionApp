using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Services
{
    public class LogService : ILogService
    {
        public void LogException(
            Exception thrown,
            OptionalParameterDemarc optionalParameterDemarc = null,
            [CallerMemberName] string caller = "")
        {
            var message = new StringBuilder();
            message.AppendLine($"Unhandled exception from {caller} at {DateTime.Now}");

            var exception = thrown;
            while (exception != null)
            {
                message.AppendLine(exception.Message);
                message.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            message.AppendLine();

            var fileName = Path.Combine(FileHelper.GetDataFolder(), "app.log");
            File.AppendAllText(fileName, message.ToString());
        }
    }
}
