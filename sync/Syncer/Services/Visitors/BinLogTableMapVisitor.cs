﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;

namespace Syncer.Services.Visitors
{
    public class BinLogTableMapVisitor : IBinLogEventVisitor
    {
        private readonly ILogger<BinLogTableMapVisitor> _logger;

        public BinLogTableMapVisitor(ILogger<BinLogTableMapVisitor> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is TableMapEvent;
        }

        public Task Handle(IBinlogEvent binlogEvent, ExecutionContext executionContext)
        {
            var tableMapEvent = binlogEvent as TableMapEvent;
            Debug.Assert(tableMapEvent != null, nameof(tableMapEvent) + " != null");


            if (executionContext.OrdinalConfiguration.Tables.Any(table => table.Id == tableMapEvent.TableId))
            {
                return Task.CompletedTask;
            }

            // add new table context

            var tableConfiguration = new TableConfiguration
            {
                Id = tableMapEvent.TableId,
                Name = tableMapEvent.TableName,
                DatabaseName = tableMapEvent.DatabaseName,
                Columns = new List<ColumnConfiguration>()
            };

            var columnOrdinalId = 0;
            foreach (var column in tableMapEvent.TableMetadata.ColumnNames)
            {
                tableConfiguration.Columns.Add(new ColumnConfiguration
                {
                    Name = column,
                    Id = columnOrdinalId
                });

                columnOrdinalId++;
            }

            executionContext.OrdinalConfiguration.Tables.Add(tableConfiguration);

            _logger.LogInformation($"Acquired table metadata for {tableMapEvent.TableName}");

            return Task.CompletedTask;
        }
    }
}