using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.App.Services
{
    public interface IAppLaunchService
    {
        Task OnLaunchAsync();

        Task OnActivateAsync();
    }
}
