﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Syncer.Configuration;
using Syncer.Configuration.Utils;
using Syncer.Contracts;
using Syncer.Services;

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
            var applicationConfiguration = ConfigurationFactory.CreateConfiguration();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(applicationConfiguration).CreateLogger();

            serviceCollection.Configure<CanalConfiguration>(
                applicationConfiguration.GetSection(nameof(CanalConfiguration)));

            serviceCollection.AddSingleton(applicationConfiguration);

            serviceCollection.AddLogging(configuration => configuration.AddConsole().AddSerilog());

            serviceCollection.AddTransient<IBinLogSyncService, BinLogSyncService>();
            serviceCollection.AddTransient<ICanalConnector, CanalConnectorService>();
        }
    }
}