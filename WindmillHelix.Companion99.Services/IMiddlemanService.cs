using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Services
{
    public interface IMiddlemanService
    {
        Task EnsureMiddlemanDownloadedAsync();

        Task StartMiddlemanAsync();

        void StopMiddleman();

        string GetCurrentHostName();

        void SetHostName(string hostName);

        bool IsMiddlemanRunning();
    }
}
