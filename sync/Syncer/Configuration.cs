using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Services;
using Syncer.Services.Handlers;
using Syncer.Services.Visitors;

namespace Syncer
{
    internal class AppConfiguration
    {
        internal static ServiceProvider InitializeServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDeleteHandler, TestDocumentDeleteHandler>();
            serviceCollection.AddTransient<ICreateHandler, TestDocumentCreateHandler>();
            serviceCollection.AddTransient<IUpdateHandler, TestDocumentUpdateHandler>();

            serviceCollection.AddTransient<ICreateHandlerFactory, CreateHandlerFactory>();
            serviceCollection.AddTransient<IDeleteHandlerFactory, DeleteHandlerFactory>();
            serviceCollection.AddTransient<IUpdateHandlerFactory, UpdateHandlerFactory>();
            serviceCollection.AddTransient<IHandlerFence, HandlerFence>();

            var applicationConfiguration = ConfigurationFactory.CreateConfiguration();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(applicationConfiguration).CreateLogger();

            serviceCollection.Configure<DatabaseConfiguration>(
                applicationConfiguration.GetSection(nameof(DatabaseConfiguration)));

            serviceCollection.Configure<BinLogConfiguration>(
                applicationConfiguration.GetSection(nameof(BinLogConfiguration)));

            serviceCollection.Configure<ElasticSearchConfiguration>(
                applicationConfiguration.GetSection(nameof(ElasticSearchConfiguration)));

            serviceCollection.AddSingleton(applicationConfiguration);

            serviceCollection.AddLogging(configuration => configuration.AddConsole().AddSerilog());

            serviceCollection.AddTransient<IBinLogSyncService, BinLogSyncService>();


            serviceCollection.AddTransient<IBinLogEventVisitor, BinLogTableMapVisitor>();
            serviceCollection.AddTransient<IBinLogEventVisitor, BinLogCreateVisitor>();
            serviceCollection.AddTransient<IBinLogEventVisitor, BinLogUpdateVisitor>(); 
            serviceCollection.AddTransient<IBinLogEventVisitor, BinLogDeleteVisitor>();
            serviceCollection.AddTransient<IBinLogEventVisitor, BinLogPositionVisitor>();

            serviceCollection.AddTransient<ICdcClientProvider, CdcClientProvider>();
        }
    }
}
