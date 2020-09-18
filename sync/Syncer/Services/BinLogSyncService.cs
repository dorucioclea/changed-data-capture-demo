using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.Alibaba.Otter.Canal.Protocol;
using Microsoft.Extensions.Logging;
using Syncer.Contracts;
using Syncer.Entities;

namespace Syncer.Services
{
    public class BinLogSyncService : IBinLogSyncService
    {
        private readonly ILogger<BinLogSyncService> _logger;
        private readonly ICanalConnector _canalConnector;

        public BinLogSyncService(ILogger<BinLogSyncService> logger, ICanalConnector canalConnector)
        {
            _logger = logger;
            _canalConnector = canalConnector;
        }

        public void Initialize()
        {
            _logger.LogInformation("Initializing Canal Client");

            _canalConnector.Connect();

            _logger.LogInformation("Canal Client has been initialized successfully");
        }

        public async ValueTask<SyncStatus> Sync()
        {
            await Task.Delay(10);
            var newMessages = _canalConnector.Get();

            var batchId = newMessages.Id;

            if (batchId == -1 || !newMessages.Entries.Any())
            {
                return SyncStatus.NoMessages();
            }

            PrintEntry(newMessages.Entries);

            return new SyncStatus
            {
                Success = true,
                HasMessages = true
            };
        }

        private void PrintEntry(IEnumerable<Entry> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.EntryType == EntryType.Transactionbegin || entry.EntryType == EntryType.Transactionend)
                {
                    continue;
                }

                RowChange rowChange = null;

                try
                {
                    rowChange = RowChange.Parser.ParseFrom(entry.StoreValue);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }

                if (rowChange == null) continue;


                var eventType = rowChange.EventType;
                _logger.LogInformation(
                    $"================> binlog[{entry.Header.LogfileName}:{entry.Header.LogfileOffset}] , name[{entry.Header.SchemaName},{entry.Header.TableName}] , eventType :{eventType}");

                foreach (var rowData in rowChange.RowDatas)
                {
                    switch (eventType)
                    {
                        case EventType.Delete:
                            PrintColumn(rowData.BeforeColumns.ToList());
                            break;
                        case EventType.Insert:
                            PrintColumn(rowData.AfterColumns.ToList());
                            break;
                        default:
                            _logger.LogInformation("-------> before");
                            PrintColumn(rowData.BeforeColumns.ToList());
                            _logger.LogInformation("-------> after");
                            PrintColumn(rowData.AfterColumns.ToList());
                            break;
                    }
                }
            }
        }

        private void PrintColumn(List<Column> columns)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var column in columns)
            {
                builder.AppendLine($"{column.Name} ： {column.Value}  update=  {column.Updated}");
            }

            _logger.LogInformation(builder.ToString());
        }
    }
}
