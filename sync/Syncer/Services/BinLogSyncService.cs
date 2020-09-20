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
        private readonly ExecutionContext _executionContext = new ExecutionContext();

        public BinLogSyncService(ILogger<BinLogSyncService> logger,
            IOptions<DatabaseConfiguration> databaseConfiguration,
            IEnumerable<IBinLogEventVisitor> binLogEventVisitors)
        {
            _logger = logger;
            _databaseConfiguration = databaseConfiguration;
            _binLogEventVisitors = binLogEventVisitors;
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing Mariadb CDC");

            _binlogClient = new BinlogClient(options =>
            {
                options.Port = _databaseConfiguration.Value.ServerPort;
                options.Hostname = _databaseConfiguration.Value.ServerAddress;
                // options.Database = _databaseConfiguration.Value.Database;
                options.Username = _databaseConfiguration.Value.Credentials.UserName;
                options.Password = _databaseConfiguration.Value.Credentials.Password;
                options.SslMode = SslMode.DISABLED;
                options.HeartbeatInterval = TimeSpan.FromSeconds(10);
                options.Blocking = true;

                // // Start replication from MariaDB GTID. Recommended.
                // options.Binlog = BinlogOptions.FromGtid(GtidList.Parse("0-1-270"));
                //
                // // Start replication from MySQL GTID. Recommended.
                // var gtidSet = "d4c17f0c-4f11-11ea-93e3-325d3e1cd1c8:1-107, f442510a-2881-11ea-b1dd-27916133dbb2:1-7";
                // options.Binlog = BinlogOptions.FromGtid(GtidSet.Parse(gtidSet));
                //
                // // Start replication from the master binlog filename and position
                // options.Binlog = BinlogOptions.FromPosition("mysql-bin.000008", 195);
                //
                // // Start replication from the master last binlog filename and position.
                // options.Binlog = BinlogOptions.FromEnd();

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
                //TODO: Record state so we can start from there the next time

                var _ = _binlogClient.State;

                foreach (var visitor in _binLogEventVisitors)
                {
                    if (visitor.CanHandle(binlogEvent))
                    {
                        await visitor.Handle(binlogEvent, _executionContext);
                    }
                }
            });

            return new SyncStatus();
        }
    }
}
