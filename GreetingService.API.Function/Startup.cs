using Azure.Messaging.ServiceBus;
using GreetingService.API.Function.Authentication;
using GreetingService.Core.Interfaces;
using GreetingService.Infrastructure;
using GreetingService.Infrastructure.GreetingRepository;
using GreetingService.Infrastructure.UserService;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Azure.Identity;
using System;
using Microsoft.Azure.KeyVault;


[assembly: FunctionsStartup(typeof(GreetingService.API.Function.Startup))]
namespace GreetingService.API.Function
{
    public class Startup : FunctionsStartup
    {
        //public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        //{
        //    builder.ConfigurationBuilder.AddAzureKeyVault(Environment.GetEnvironmentVariable("KeyVaultUri"));
        //}
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var builtConfig = builder.ConfigurationBuilder.Build();
            var keyVaultEndpoint = builtConfig["AzureKeyVaultEndpoint"];
            builder.ConfigurationBuilder
                        .AddAzureKeyVault(keyVaultEndpoint);
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            builder.Services.AddHttpClient();
            builder.Services.AddLogging();



            //Create a Serilog logger and register it as a logger
            //Get the Azure Storage Account connection string from our IConfiguration
            builder.Services.AddLogging(c =>
            {
                var connectionString = config["LoggingStorageAccount"];
                if (string.IsNullOrWhiteSpace(connectionString))
                    return;

                var logName = $"Serilog.log";
               //var logName = $"{Assembly.GetCallingAssembly().GetName().Name}.log";
                var logger = new LoggerConfiguration()
                                    //.WriteTo.AzureBlobStorage(connectionString,
                                    //                          restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                                    //                          storageFileName: "{yyyy}/{MM}/{dd}/" + logName,
                                    //                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}")
                                    
                                    .CreateLogger();

                c.AddSerilog(logger,true);
            });


            //builder.Services.AddScoped<IGreetingRepository, FileGreetingRepository>(c =>
            //{
            //    var config = c.GetService<IConfiguration>();
            //    return new FileGreetingRepository(config["FileRepositoryFilePath"]);
            //});
            //builder.Services.AddSingleton<IGreetingRepository, MemoryGreetingRepository>();
            //builder.Services.AddScoped<IGreetingRepository, BlobGreetingRepository>();
            builder.Services.AddScoped<IGreetingRepository, SqlGreetingRepository>();

            //builder.Services.AddScoped<IUserService, BlobUserService>();
            builder.Services.AddScoped<IUserService, SqlUserService>();
            //builder.Services.AddScoped<IUserService, AppSettingsUserService>();
            builder.Services.AddScoped<IAuthHandler, BasicAuthHandler>();
            builder.Services.AddScoped<IInvoiceService, SqlInvoiceService>();

            builder.Services.AddScoped<IMessagingService, ServiceBusMessagingService>();
            builder.Services.AddScoped<IApprovalService, TeamsApprovalService>();


            builder.Services.AddDbContext<GreetingDbContext>(options =>
            {
                options.UseSqlServer(config["GreetingDbConnectionString"]);
            });


            builder.Services.AddSingleton(c =>
            {
                var serviceBusClient = new ServiceBusClient(config["ServiceBusConnectionString"]);      //remember to add this connection to the application configuration
                return serviceBusClient.CreateSender("main");
            });


        }
    }
}