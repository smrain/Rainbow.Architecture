using Autofac;
using Rainbow.Architecture.Infrastructure.Idempotency;
using Rainbow.Extensions.EventBus.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rainbow.Architecture.API.Infrastructure.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Get executing assemblies
            var executingAssemblies = GetAllAssemblies();

            // Register class name endswith "Repository" or "Queries"
            builder.RegisterAssemblyTypes(executingAssemblies)
                .Where(@class => @class.Name.EndsWith("Repository") | @class.Name.EndsWith("Queries"))
                .PublicOnly()
                .Where(type => type.IsClass)
                //.Except<BaseEntityRepository>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            // Register type IRequestManager
            builder.RegisterType<RequestManager>().As<IRequestManager>().InstancePerLifetimeScope();

            // Register closed types of IIntegrationEventHandler<>
            builder.RegisterAssemblyTypes(executingAssemblies).AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }

        private static Assembly[] GetAllAssemblies()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            List<Assembly> assemblies = executingAssembly.GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Where(lib => lib.FullName.Split(',').First().EndsWith(".Domain") | lib.FullName.Split(',').First().EndsWith(".Infrastructure") | lib.FullName.Split(',').First().EndsWith(".API"))
                .ToList();
            Assembly assembly = Assembly.GetEntryAssembly();
            assemblies.Add(assembly);
            return assemblies.ToArray();
        }
    }
}
