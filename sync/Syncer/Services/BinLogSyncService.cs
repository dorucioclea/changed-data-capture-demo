using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlCdc;
using MySqlCdc.Constants;
using MySqlCdc.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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

        public BinLogSyncService(ILogger<BinLogSyncService> logger, IOptions<DatabaseConfiguration> databaseConfiguration)
        {
            _logger = logger;
            _databaseConfiguration = databaseConfiguration;
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
            await _binlogClient.ReplicateAsync(async (binlogEvent) =>
            {
                var state = _binlogClient.State;

                if (binlogEvent is TableMapEvent tableMap)
                {
                    HandleTableMapEvent(tableMap);
                }
                else if (binlogEvent is WriteRowsEvent writeRows)
                {
                    HandleWriteRowsEvent(writeRows);
                }
                else if (binlogEvent is UpdateRowsEvent updateRows)
                {
                    HandleUpdateRowsEvent(updateRows);
                }
                else if (binlogEvent is DeleteRowsEvent deleteRows)
                {
                    HandleDeleteRowsEvent(deleteRows);
                }
                else PrintEventAsync(binlogEvent);
            });

            return new SyncStatus();
        }

            private void PrintEventAsync(IBinlogEvent binlogEvent)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(binlogEvent, Formatting.Indented,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            Converters = new List<JsonConverter> { new StringEnumConverter() }
                        });


                    _logger.LogInformation(json);
                }
                catch (Exception exception)
                {

                }
            }

        private void HandleTableMapEvent(TableMapEvent tableMap)
        {
            Console.WriteLine($"Processing {tableMap.DatabaseName}.{tableMap.TableName}");
             PrintEventAsync(tableMap);
        }

        private void HandleWriteRowsEvent(WriteRowsEvent writeRows)
        {
            Console.WriteLine($"{writeRows.Rows.Count} rows were written");
            PrintEventAsync(writeRows);

            foreach (var row in writeRows.Rows)
            {
                // Do something
            }
        }

        private void HandleUpdateRowsEvent(UpdateRowsEvent updatedRows)
        {
            Console.WriteLine($"{updatedRows.Rows.Count} rows were updated");
            PrintEventAsync(updatedRows);

            foreach (var row in updatedRows.Rows)
            {
                var rowBeforeUpdate = row.BeforeUpdate;
                var rowAfterUpdate = row.AfterUpdate;
                // Do something
            }
        }

        private void HandleDeleteRowsEvent(DeleteRowsEvent deleteRows)
        {
            Console.WriteLine($"{deleteRows.Rows.Count} rows were deleted");
            PrintEventAsync(deleteRows);

            foreach (var row in deleteRows.Rows)
            {
                // Do something
            }
        }
    }
}
