using Autofac;
using Employee.API.Infrastructure.IntegrationEvents.Events;
using EventBus.Abstractions;

using System.Reflection;

namespace Employee.API.Infrastructure.AutofacModules
{

    public class ApplicationModule
        : Autofac.Module
    {

        public string QueriesConnectionString { get; }

        public ApplicationModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterAssemblyTypes(typeof(EmployeeCreatedSuccessfullyEventHandler).GetTypeInfo().Assembly)
 .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
            builder.RegisterAssemblyTypes(typeof(EmployeeCreatedFailedEventHandler).GetTypeInfo().Assembly)
 .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}
