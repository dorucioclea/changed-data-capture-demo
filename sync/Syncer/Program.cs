using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Syncer.Contracts;
using Syncer.Services;
using Syncer.Utils;

namespace Syncer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            await using var serviceProvider = serviceCollection.BuildServiceProvider();
            var binLogSyncService = serviceProvider.GetService<IBinLogSyncService>();

            await binLogSyncService.Sync();

            var logger = serviceProvider.GetService<ILogger<Program>>();

            logger.LogInformation("Hello World from docker!");
        }
     
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var applicationConfiguration = ConfigurationFactory.CreateConfiguration();

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(applicationConfiguration).CreateLogger();

            serviceCollection.AddSingleton(applicationConfiguration);

            serviceCollection.AddLogging(configuration => configuration.AddConsole().AddSerilog());

            serviceCollection.AddTransient<IBinLogSyncService, BinLogSyncService>();
        }
    }
}
