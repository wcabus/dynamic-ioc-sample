using System;
using System.IO;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

[assembly: OwinStartup(typeof(Sample.Startup))]

namespace Sample
{
    public sealed class Startup
    {
        private static AppDomain _domain;
        private static readonly object DomainLock = new object();

        private static bool _assemblySwitch = true;

        private static readonly AppDomainSetup Domaininfo = new AppDomainSetup
        {
            ApplicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin")
        };

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            SetupFormatters(config);
            config.MapHttpAttributeRoutes();

            ConfigureDependencies(config);

            app.UseWebApi(config);
        }

        private void SetupFormatters(HttpConfiguration config)
        {
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        }

        public static void ConfigureDependencies(HttpConfiguration config)
        {
            ContainerBuilder builder;
            lock (DomainLock)
            {
                if (_domain != null)
                {
                    AppDomain.Unload(_domain);
                }

                _domain = AppDomain.CreateDomain("Sample.IOC", AppDomain.CurrentDomain.Evidence, Domaininfo);

                var asmName = AssemblyName.GetAssemblyName(
                    Path.Combine(Domaininfo.ApplicationBase, 
                    _assemblySwitch 
                        ? "Sample.Repositories.dll" 
                        : "Sample.Repositories.Alternative.dll"));

                var assembly = _domain.Load(asmName.FullName);
                _assemblySwitch = !_assemblySwitch;

                builder = new ContainerBuilder();
                builder.RegisterAssemblyTypes(assembly)
                    .AsImplementedInterfaces()
                    .InstancePerRequest();

                builder.RegisterApiControllers(typeof(Startup).Assembly);
            }

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
