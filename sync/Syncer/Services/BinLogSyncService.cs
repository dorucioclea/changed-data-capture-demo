using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Entities;
using ExecutionContext = Syncer.Configuration.ExecutionContext;

namespace Syncer.Services
{
    public class BinLogSyncService : IBinLogSyncService
    {
        private const int NumberOfRetries = 5;

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

            // Define our policy:
            var policy = Policy.Handle<Exception>().WaitAndRetryAsync(
                NumberOfRetries, // Retry
                attempt => TimeSpan.FromMilliseconds(200), // Wait 200ms between each try.
                (exception, calculatedWaitDuration) => // Capture some info for logging!
                {
                    // This is your new exception handler! 
                    // Tell the user what they've won!

                    _logger.LogError($"Could not replicate - exception {exception.Message}");
                });

            // Do the following until a key is pressed
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Retry the following call according to the policy - 3 times.
                    await policy.ExecuteAsync(async token =>
                    {
                        // This code is executed within the Policy 

                        await binLogClient.ReplicateAsync(async @event =>
                        {
                            foreach (var visitor in _binLogEventVisitors)
                            {
                                if (visitor.CanHandle(@event))
                                    await visitor.Handle(new EventInfo { Event = @event, Options = binLogClient.State }, _executionContext);
                            }
                        }, token);

                    }, cancellationToken);
                }
                catch (Exception exception)
                {
                    throw new AggregateException($"Sync request eventually failed after {NumberOfRetries} retries", exception);
                }
            }

            _logger.LogInformation("Sync process stopping");
        }
    }
}
