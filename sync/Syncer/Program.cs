using System;
using System.Threading;
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
            await using var serviceProvider = AppConfiguration.InitializeServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();

            try
            {
                var tokenSource = new CancellationTokenSource();

                var binLogSyncService = serviceProvider.GetService<IBinLogSyncService>();
                await binLogSyncService.Sync(tokenSource.Token);
            }
            catch (Exception exception)
            {
                logger.LogError($"TopMost error: {exception.Message}");
            }
        }
    }
}
