using Autofac;
using FluentValidation;
using MediatR;
using Rainbow.Architecture.API.Application.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rainbow.Architecture.API.Infrastructure.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Get executing assemblies
            var executingAssemblies = GetAllAssemblies();

            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(executingAssemblies).AsClosedTypesOf(typeof(IRequestHandler<,>)); ;

            // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
            builder.RegisterAssemblyTypes(executingAssemblies).AsClosedTypesOf(typeof(INotificationHandler<>)); ;

            // Register the Command's Validators (Validators based on FluentValidation library)
            builder.RegisterAssemblyTypes(executingAssemblies).Where(t => t.IsClosedTypeOf(typeof(IValidator<>))).AsImplementedInterfaces();


            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
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
