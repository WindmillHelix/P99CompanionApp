using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Common;
using WindmillHelix.Companion99.Services.Interop;

namespace WindmillHelix.Companion99.Services
{
    public class MiddlemanService : IMiddlemanService
    {
        private readonly IConfigurationService _configurationService;

        public MiddlemanService(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        public async Task EnsureMiddlemanDownloadedAsync()
        {
            var path = GetFilePath();
            if(File.Exists(path))
            {
                return;
            }

            var client = new HttpClient();
            var zipBytes = await client.GetByteArrayAsync(MiddlemanConstants.DownloadUrl);
            var stream = new MemoryStream(zipBytes);
            var zip = new ZipArchive(stream);
            var entry = zip.GetEntry(MiddlemanConstants.FileName);
            entry.ExtractToFile(path);
        }

        public string GetCurrentHostName()
        {
            var hostFile = Path.Combine(_configurationService.EverQuestFolder, "eqhost.txt");

            var valueBuilder = new StringBuilder(255);
            IniFile.GetPrivateProfileString("LoginServer", "Host", string.Empty, valueBuilder, 255, hostFile);
            return valueBuilder.ToString();
        }

        public void SetHostName(string hostName)
        {
            var hostFile = Path.Combine(_configurationService.EverQuestFolder, "eqhost.txt");
            IniFile.WritePrivateProfileString("LoginServer", "Host", hostName, hostFile);
        }

        public bool IsMiddlemanRunning()
        {
            var processes = Process.GetProcessesByName(MiddlemanConstants.ProcessName);
            var result = processes.Length > 0;
            return result;
        }

        public async Task StartMiddlemanAsync()
        {
            if(IsMiddlemanRunning())
            {
                return;
            }

            await EnsureMiddlemanDownloadedAsync();

            var path = GetFilePath();
            var start = new ProcessStartInfo(path);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;

            var process = Process.Start(start);
        }

        public void StopMiddleman()
        {
            var processes = Process.GetProcessesByName(MiddlemanConstants.ProcessName);
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception thrown)
                {
                    // todo: log
                }
            }
        }

        private string GetFilePath()
        {
            var path = Path.Combine(FileHelper.GetAppFolder(), MiddlemanConstants.FileName);
            return path;
        }
    }
}
