using Employee.API.IntegrationEvents;
using Employee.API.Repositories.Implementations;
using Employee.API.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceImplementation;
using Upscript.Services.Employee.API.Infrastructure.BusnessLogic.ServiceInterface;

namespace Upscript.Services.Employee.API.Common
{
    public static class ServiceExtentions
    {
        /// <summary>
        /// Extention method to add custom services.
        /// </summary>
        /// <param name="services"></param>
        public static void AddCustomServices(this IServiceCollection services)
        {
            //DI for serivce layer
            services.AddTransient<IEmployeeService, EmployeeService>();

            //DI for repository layer
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            

            //DI for Events
            services.AddTransient<IEmployeeIntegrationEventService, EmployeeIntegrationEventService>();
        }
    }
}
