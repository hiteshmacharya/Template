using Autofac;
using Autofac.Extensions.DependencyInjection;
using Employee.API.Infrastructure.AutofacModules;
using Employee.API.Infrastructure.IntegrationEvents.Events;
using Employee.API.IntegrationEvents;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using HealthChecks.UI.Client;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System;
using System.Data.Common;
using System.Reflection;
using Upscript.Services.Employee.API.Common;
using Upscript.Services.Employee.API.Middlewares;
using UpScript.Services.Employee.API.Infrastructure;

namespace Upscript.Services.Employee.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();


            //UnComment when SQS Required
            //var appSettingsSection = Configuration.GetSection("ServiceConfiguration");
            //services.AddAWSService<IAmazonSQS>();
            //services.Configure<ServiceConfiguration>(appSettingsSection);
            //services.AddTransient<IAWSSQSService, AWSSQSService>();
            //services.AddTransient<IAWSSQSHelper, AWSSQSHelper>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee.API", Version = "v1" });
            });
            services.AddCustomHealthCheck(Configuration);
            services.AddCustomIntegrations(Configuration);
            services.AddEventBus(Configuration);

            string connectionString = Configuration.GetConnectionString("EmployeeContext");
            services.AddDbContextPool<EmployeeContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            string integrationEventLogConnection = Configuration.GetConnectionString("IntegrationEventLogConnection");
            services.AddDbContextPool<IntegrationEventLogContext>(options =>
            {
                options.UseMySql(integrationEventLogConnection, ServerVersion.AutoDetect(integrationEventLogConnection),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            });

            ///Adding Custom extention method for dependency injection.
            services.AddCustomServices();

            
            services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
                sp => (DbConnection c) => new IntegrationEventLogService(c));

            //configure autofac

            var container = new ContainerBuilder();
            container.Populate(services);

             
            container.RegisterModule(new ApplicationModule(Configuration["ConnectionString"]));

            return new AutofacServiceProvider(container.Build());
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, EmployeeContext db, IntegrationEventLogContext integrationDb)
        {

            loggerFactory.AddFile("Logs/Employee-API-{Date}.txt");
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API v1"));

            db.Database.Migrate();
            integrationDb.Database.Migrate();

            //Configuring global exception handler middleware
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            var pathBase = Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
            ConfigureEventBus(app);
        }

        private void ConfigureEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<EventBus.Abstractions.IEventBus>();

            eventBus.Subscribe<EmployeeCreatedSuccessfullyEvent, IIntegrationEventHandler<EmployeeCreatedSuccessfullyEvent>>();
            eventBus.Subscribe<EmployeeCreatedFailedEvent, IIntegrationEventHandler<EmployeeCreatedFailedEvent>>();

        }
    }
    static class CustomExtensionsMethods
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());
            return services;
        }
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
            {
                var subscriptionClientName = configuration["SubscriptionClientName"];
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();

                var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ.EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope,
                    eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }

        public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddTransient<IIdentityService, IdentityService>();
            //services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
            //    sp => (DbConnection c) => new IntegrationEventLogService(c));

            //services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

                 services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


                    var factory = new ConnectionFactory()
                    {
                        HostName = configuration["EventBusConnection"],
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                    {
                        factory.UserName = configuration["EventBusUserName"];
                    }

                    if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                    {
                        factory.Password = configuration["EventBusPassword"];
                    }

                    var retryCount = 5;
                    if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBusRetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });

            return services;
        }
    }
}