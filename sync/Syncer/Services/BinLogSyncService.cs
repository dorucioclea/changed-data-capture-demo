using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Syncer.Configuration;
using Syncer.Contracts;
using ExecutionContext = Syncer.Configuration.ExecutionContext;

namespace Syncer.Services
{
    public class BinLogSyncService : IBinLogSyncService
    {
        private readonly ILogger<BinLogSyncService> _logger;
        private readonly IOptions<DatabaseConfiguration> _databaseConfiguration;
        private readonly IOptions<BinLogConfiguration> _binLogConfiguration;
        private readonly ICdcClientProvider _cdcClientProvider;
        private readonly IEnumerable<IBinLogEventVisitor> _binLogEventVisitors;
        private readonly ExecutionContext _executionContext;

        public BinLogSyncService(ILogger<BinLogSyncService> logger,
            IOptions<DatabaseConfiguration> databaseConfiguration,
            IOptions<BinLogConfiguration> binLogConfiguration,
            ICdcClientProvider cdcClientProvider,
            IEnumerable<IBinLogEventVisitor> binLogEventVisitors)
        {
            _logger = logger;
            _databaseConfiguration = databaseConfiguration;
            _binLogConfiguration = binLogConfiguration;
            _cdcClientProvider = cdcClientProvider;
            _binLogEventVisitors = binLogEventVisitors;

            _executionContext = new ExecutionContext
            {
                BinLogLedgerPath = binLogConfiguration.Value.FilePath
            };
        }


        public async ValueTask Sync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sync process starting ");

            var binLogClient = _cdcClientProvider.Provide(_databaseConfiguration.Value, _binLogConfiguration.Value);

            await binLogClient.ReplicateAsync(async @event =>
            {
                foreach (var visitor in _binLogEventVisitors)
                {
                    if (visitor.CanHandle(@event))
                        await visitor.Handle(new EventInfo { Event = @event, Options = binLogClient.State }, _executionContext);
                }
            }, cancellationToken);

            _logger.LogInformation("Sync process stopping");
        }
    }
}
