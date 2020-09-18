using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncer.Contracts;

namespace Syncer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            await using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                var binLogSyncService = serviceProvider.GetService<IBinLogSyncService>();

                await binLogSyncService.Sync();
            }

            Console.WriteLine("Hello World from docker!");
        }


        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(configuration => configuration.AddConsole());

            serviceCollection.AddTransient<IBinLogSyncService, IBinLogSyncService>();
        }
    }
}
