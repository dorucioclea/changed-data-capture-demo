using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlCdc;
using MySqlCdc.Constants;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Entities;

namespace Syncer.Services
{
    public class CdcClientProvider : ICdcClientProvider
    {
        private readonly ILogger<CdcClientProvider> _logger;

        public CdcClientProvider(ILogger<CdcClientProvider> logger)
        {
            _logger = logger;
        }

        public BinlogClient Provide(DatabaseConfiguration databaseConfiguration, BinLogConfiguration binLogConfiguration)
        {
            _logger.LogInformation("Initializing Mariadb CDC");

            var binLogClient = new BinlogClient(async options =>
            {
                options.Port = databaseConfiguration.ServerPort;
                options.Hostname = databaseConfiguration.ServerAddress;
                options.Database = databaseConfiguration.Database;
                options.Username = databaseConfiguration.Credentials.UserName;
                options.Password = databaseConfiguration.Credentials.Password;
                options.SslMode = SslMode.DISABLED;
                options.HeartbeatInterval = TimeSpan.FromSeconds(10);
                options.ServerId = databaseConfiguration.ServerId;
                options.Blocking = true;

                // Start replication from the master first available(not purged) binlog filename and position.

                var binLog = await GetBinLog(binLogConfiguration);

                if (binLog != null)
                {
                    _logger.LogInformation($"Starting from {binLog.FileName} at position {binLog.Position}");
                    options.Binlog = BinlogOptions.FromPosition(binLog.FileName, binLog.Position);
                }
                else
                {
                    options.Binlog = BinlogOptions.FromStart();
                }
            });


            _logger.LogInformation("CDC Client has been initialized successfully");

            return binLogClient;
        }

        private async ValueTask<BinLog> GetBinLog(BinLogConfiguration binLogConfiguration)
        {
            if (!binLogConfiguration.AutoSave)
                return null;

            _logger.LogInformation($"Binlog has AutoSave ON. Attempting to load last known binlog position from {binLogConfiguration.FilePath}");

            var ledgerFileContent = await GetBinLogLedgerContent(binLogConfiguration.FilePath);

            return ledgerFileContent != null ? new BinLog(ledgerFileContent) : null;
        }

        private async ValueTask<string> GetBinLogLedgerContent(string filePath)
        {
            string binLogContent = null;

            try
            {
                binLogContent = await File.ReadAllTextAsync(filePath);
            }
            catch (Exception)
            {
                _logger.LogError("Ledger file not found. Starting from scratch.");
            }

            return binLogContent;
        }
    }
}
