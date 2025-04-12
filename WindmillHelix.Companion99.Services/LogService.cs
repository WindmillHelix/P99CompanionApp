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
        public void Log(
            string message, 
            OptionalParameterDemarc optionalParameterDemarc = null, 
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            WriteLog(message, callerMemberName, callerFilePath);
        }

        public void LogException(
            Exception thrown,
            OptionalParameterDemarc optionalParameterDemarc = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            var message = new StringBuilder();
            message.AppendLine($"Exception stack trace");

            var exception = thrown;
            while (exception != null)
            {
                message.AppendLine(exception.Message);
                message.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }

            message.AppendLine();
            WriteLog(message.ToString(), callerMemberName, callerFilePath);
        }

        private void WriteLog(string message, string callerMemberName, string callerFilePath)
        {
            const string dateFormatString = "yyyy-MM-dd HH:mm:ss.fff";
            var builder = new StringBuilder();

            var path = callerFilePath;
            var toFind = @"\WindmillHelix.Companion99.";
            var index = path.IndexOf(toFind);
            if (index > -1)
            {
                path = path.Substring(index + 1);
            }

            builder.AppendLine($"[{DateTime.Now.ToString(dateFormatString)}] [{path}].[{callerMemberName}]: {message}");

            var fileName = Path.Combine(FileHelper.GetDataFolder(), "app.log");
            File.AppendAllText(fileName, builder.ToString());
        }
    }
}
