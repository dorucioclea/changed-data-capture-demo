using System;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Serilog;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Elasticsearch;
using Syncer.Elasticsearch.Abstractions;
using Syncer.Elasticsearch.Documents;
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
            serviceCollection.AddTransient<ICreateHandlerFactory, CreateHandlerFactory>();
            serviceCollection.AddTransient<IDeleteHandlerFactory, DeleteHandlerFactory>();

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

            serviceCollection.AddTransient<IElasticsearchRepository>(provider =>
            {
                var elasticSearchConfiguration =
                    provider.GetService<IOptions<ElasticSearchConfiguration>>();

                var connectionSettings = new ConnectionSettings(new Uri(elasticSearchConfiguration.Value.Host))
                    .BasicAuthentication(elasticSearchConfiguration.Value.UserName, elasticSearchConfiguration.Value.Password)
                    .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
                    .DefaultMappingFor<TestDocument>(i => i.IndexName("test"))
                    .PrettyJson();

                var elasticClient = new ElasticClient(connectionSettings);

                var elasticSearchRepositoryAsync = new ElasticsearchRepository(elasticClient);

                return elasticSearchRepositoryAsync;
            });
        }
    }
}
