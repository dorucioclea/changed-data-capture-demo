using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Syncer.Contracts;

namespace Syncer.Services
{
    public class BinLogSyncService : IBinLogSyncService
    {
        private readonly ILogger<BinLogSyncService> _logger;

        public BinLogSyncService(ILogger<BinLogSyncService> logger)
        {
            _logger = logger;
        }

        public async ValueTask Sync()
        {
            _logger.LogInformation("Before sync");
            await Task.Delay(199);
            _logger.LogInformation("After sync");
        }
    }
}
