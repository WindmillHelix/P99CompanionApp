using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.Services.Discord.Internals;

namespace WindmillHelix.Companion99.Services.Config
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterTypes(
                ThisAssembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsInterface && x.Name.EndsWith("Service")).ToArray())
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<Discord2LogRunner>().AsSelf().SingleInstance();
            builder.RegisterType<Log2DiscordRunner>().AsSelf().SingleInstance();
        }
    }
}
