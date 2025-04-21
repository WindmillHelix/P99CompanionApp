using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindmillHelix.Companion99.App.DiscordOverlay;
using WindmillHelix.Companion99.App.Services;

namespace WindmillHelix.Companion99.App.Config
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterTypes(
                ThisAssembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsInterface && x.Name.EndsWith("Service")).ToArray())
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterTypes(
                ThisAssembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsInterface && x.Name.EndsWith("Window")).ToArray())
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<DiscordOverlayManager>().AsSelf().SingleInstance();
            builder.RegisterType<LocationSaver>().AsSelf().InstancePerDependency();
        }
    }
}