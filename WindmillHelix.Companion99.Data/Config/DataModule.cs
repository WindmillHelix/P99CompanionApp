using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindmillHelix.Companion99.Data.Config
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var suffixes = new List<string>();
            suffixes.Add("Repository");
            suffixes.Add("Service");
            suffixes.Add("Factory");
            suffixes.Add("Initializer");
            suffixes.Add("Migrator");

            builder.RegisterTypes(
                ThisAssembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsInterface && suffixes.Any(s => x.Name.EndsWith(s))).ToArray())
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
