using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Syncer.Contracts;
using Syncer.Entities;

namespace Syncer.Services
{
    public class BinLogSyncService : IBinLogSyncService
    {
        private readonly ILogger<BinLogSyncService> _logger;

        public BinLogSyncService(ILogger<BinLogSyncService> logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing Canal Client");


            _logger.LogInformation("Canal Client has been initialized successfully");
        }

        public async ValueTask<SyncStatus> Sync()
        {
            await Task.Delay(10);

            return new SyncStatus();
        }
    }
}
