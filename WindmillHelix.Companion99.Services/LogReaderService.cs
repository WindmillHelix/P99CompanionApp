using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;

namespace WindmillHelix.Companion99.Services
{
    public class LogReaderService : ILogReaderService
    {
        private object _lock = new object();
        private readonly List<ILogListener> _listeners = new List<ILogListener>();
        private readonly IConfigurationService _configurationService;
        private bool _isStarted = false;

        private string _currentLogFileName;

        public LogReaderService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public void AddListener(ILogListener listener)
        {
            lock (_lock)
            { 
                if(!_listeners.Contains(listener))
                {
                    _listeners.Add(listener);
                }
            }
        }

        public void RemoveListener(ILogListener listener)
        {
            lock (_lock)
            {
                if (_listeners.Contains(listener))
                {
                    _listeners.Remove(listener);
                }
            }
        }

        public void Start()
        {
            if(_isStarted)
            {
                return;
            }

            lock (_lock)
            {
                if (_isStarted)
                {
                    return;
                }

                var start = new ThreadStart(PollLog);
                var thread = new Thread(start);

                thread.Start();
                _isStarted = true;
            }
        }

        private void PollLog()
        {
            Thread.CurrentThread.IsBackground = true;
            FileStream logFile = null;
            StreamReader reader = null;
            string characterName = null;
            string serverName = null;

            while (true)
            {
                var latest = GetLatestLogFile();
                if (latest.FullName != _currentLogFileName)
                {
                    if(reader != null)
                    {
                        reader.Close();
                        reader.Dispose();

                        logFile.Close();
                        logFile.Dispose();
                    }

                    logFile = OpenLogFile(latest.FullName);
                    reader = new StreamReader(logFile);
                    var bufferLength = -1 * Math.Min(logFile.Length, 32 * 1024);
                    reader.BaseStream.Seek(bufferLength, SeekOrigin.End);
                    var buffer = reader.ReadToEnd();
                    var bufferLines = buffer.Split('\n').Select(x => x.Trim()).ToList();

                    var fileNameParts = latest.Name.Split('_');
                    characterName = fileNameParts[1];
                    serverName = fileNameParts[2].Replace(".txt", string.Empty);

                    _currentLogFileName = latest.FullName;

                    const string welcomeMessage = "] Welcome to EverQuest!";
                    var welcomeIndex = bufferLines.FindLastIndex(x => x.EndsWith(welcomeMessage));

                    if (welcomeIndex != -1)
                    {
                        for (int i = welcomeIndex; i < bufferLines.Count; i++)
                        {
                            var line = bufferLines[i];
                            DispatchLine(serverName, characterName, line, x => x.Item1 > DateTime.Now.AddSeconds(-60));
                        }
                    }
                }

                if (reader != null)
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        DispatchLine(serverName, characterName, line);
                        line = reader.ReadLine();
                    }
                }
                else
                {

                }

                Thread.Sleep(200);
            }
        }

        private void DispatchLine(
            string serverName, 
            string characterName, 
            string line, 
            Func<Tuple<DateTime, string>, bool> filter = null)
        {
            var parsed = ParseLine(line);
            if (parsed != null)
            {
                if(filter != null)
                {
                    var shouldDispatch = filter(parsed);
                    if(!shouldDispatch)
                    {
                        return;
                    }
                }

                var listeners = _listeners.ToArray();

                foreach (var listener in listeners)
                {
                    listener.HandleLogLine(serverName, characterName, parsed.Item1, parsed.Item2);
                }
            }

        }

        private Tuple<DateTime, string> ParseLine(string source)
        {
            if (string.IsNullOrEmpty(source) || !source.StartsWith("["))
            {
                return null;
            }

            var firstClosingBracket = source.IndexOf("]");
            var dateString = source.Substring(1, firstClosingBracket - 1);
            var timestamp = DateTime.ParseExact(dateString, "ddd MMM dd HH:mm:ss yyyy", null);
            var line = source.Substring(firstClosingBracket + 2);
            return new Tuple<DateTime, string>(timestamp, line);
        }

        private FileInfo GetLatestLogFile()
        {
            var logDirectory = Path.Combine(_configurationService.EverQuestFolder, "logs");
            var fileNames = Directory.GetFiles(logDirectory, "eqlog_*.txt");

            var infos = new List<FileInfo>();
            foreach (var fileName in fileNames)
            {
                var info = new FileInfo(fileName);
                infos.Add(info);
            }

            var latest = infos.OrderByDescending(x => x.LastWriteTimeUtc).First();
            return latest;
        }

        private FileStream OpenLogFile(string fullFilePath)
        {
            var latest = GetLatestLogFile();
            var stream = File.Open(latest.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return stream;
        }
    }
}
