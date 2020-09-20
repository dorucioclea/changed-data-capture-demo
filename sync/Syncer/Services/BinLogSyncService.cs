using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlCdc;
using MySqlCdc.Constants;
using MySqlCdc.Events;
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
        private readonly IOptions<BinLogConfiguration> _binLogConfiguration;
        private readonly IEnumerable<IBinLogEventVisitor> _binLogEventVisitors;
        private readonly ExecutionContext _executionContext;

        public BinLogSyncService(ILogger<BinLogSyncService> logger,
            IOptions<DatabaseConfiguration> databaseConfiguration,
            IOptions<BinLogConfiguration> binLogConfiguration,
            IEnumerable<IBinLogEventVisitor> binLogEventVisitors)
        {
            _logger = logger;
            _databaseConfiguration = databaseConfiguration;
            _binLogConfiguration = binLogConfiguration;
            _binLogEventVisitors = binLogEventVisitors;

            _executionContext = new ExecutionContext
            {
                BinLogLedgerPath = binLogConfiguration.Value.FilePath
            };
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing Mariadb CDC");

            _binlogClient = new BinlogClient(async options =>
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

                if (_binLogConfiguration.Value.AutoSave)
                {
                    _logger.LogInformation($"Binlog has AutoSave ON. Attempting to load last known binlog position from {_binLogConfiguration.Value.FilePath}");

                    string binlogSavedValue = null;

                    try
                    {
                        binlogSavedValue = await File.ReadAllTextAsync(_binLogConfiguration.Value.FilePath);
                    }
                    catch (Exception)
                    {
                        _logger.LogError("Ledger file not found. Starting from scratch.");
                    }

                    if (binlogSavedValue == null)
                    {
                        _logger.LogInformation("Could not find binlog position. Starting from scratch");
                        options.Binlog = BinlogOptions.FromStart();
                    }
                    else
                    {
                        var binLog = new BinLog(binlogSavedValue);

                        _logger.LogInformation($"Starting from {binLog.FileName} at position {binLog.Position}");
                        options.Binlog = BinlogOptions.FromPosition(binLog.FileName, binLog.Position);
                    }
                }
                else
                {
                    options.Binlog = BinlogOptions.FromStart();
                }

                options.ServerId = _databaseConfiguration.Value.ServerId;
            });


            _logger.LogInformation("CDC Client has been initialized successfully");
        }

        public async ValueTask Sync() => await _binlogClient.ReplicateAsync(DoReplicate);

        private async Task DoReplicate(IBinlogEvent binLogEvent)
        {
            foreach (var visitor in _binLogEventVisitors)
            {
                if (visitor.CanHandle(binLogEvent))
                    await visitor.Handle(new EventInfo { Event = binLogEvent, Options = _binlogClient.State }, _executionContext);
            }
        } 
    }
}
