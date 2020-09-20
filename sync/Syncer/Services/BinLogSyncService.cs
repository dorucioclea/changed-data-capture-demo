using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlCdc;
using MySqlCdc.Constants;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Entities;

namespace Syncer.Services
{
    public class BinLogSyncService : IBinLogSyncService
    {
        private BinlogClient _binlogClient;
        private readonly ILogger<BinLogSyncService> _logger;
        private readonly IOptions<DatabaseConfiguration> _databaseConfiguration;
        private readonly IEnumerable<IBinLogEventVisitor> _binLogEventVisitors;
        private readonly ExecutionContext _executionContext;

        public BinLogSyncService(ILogger<BinLogSyncService> logger,
            IOptions<DatabaseConfiguration> databaseConfiguration,
            IOptions<BinLogConfiguration> binLogConfiguration,
            IEnumerable<IBinLogEventVisitor> binLogEventVisitors)
        {
            _logger = logger;
            _databaseConfiguration = databaseConfiguration;
            _binLogEventVisitors = binLogEventVisitors;

            _executionContext = new ExecutionContext
            {
                BinLogLedgerPath = binLogConfiguration.Value.FilePath
            };
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing Mariadb CDC");

            _binlogClient = new BinlogClient(options =>
            {
                options.Port = _databaseConfiguration.Value.ServerPort;
                options.Hostname = _databaseConfiguration.Value.ServerAddress;
                options.Database = _databaseConfiguration.Value.Database;
                options.Username = _databaseConfiguration.Value.Credentials.UserName;
                options.Password = _databaseConfiguration.Value.Credentials.Password;
                options.SslMode = SslMode.DISABLED;
                options.HeartbeatInterval = TimeSpan.FromSeconds(10);
                options.Blocking = true;

                // Start replication from the master first available(not purged) binlog filename and position.
                options.Binlog = BinlogOptions.FromStart();
                options.ServerId = _databaseConfiguration.Value.ServerId;
            });


            _logger.LogInformation("CDC Client has been initialized successfully");
        }

        public async ValueTask<SyncStatus> Sync()
        {
            await _binlogClient.ReplicateAsync(async binlogEvent =>
            {
                foreach (var visitor in _binLogEventVisitors)
                {
                    if (visitor.CanHandle(binlogEvent))
                        await visitor.Handle(new EventInfo { Event = binlogEvent , Options = _binlogClient.State }, _executionContext);
                }
            });

            return new SyncStatus();
        }
    }
}
