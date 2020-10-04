﻿using System;
using System.Threading.Tasks;
using ArgumentValidator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Entities;

namespace Syncer.Services.Visitors
{
    public class BinLogDeleteVisitor : BaseVisitor<BinLogDeleteVisitor>, IBinLogEventVisitor
    {
        public BinLogDeleteVisitor(IOptions<DatabaseConfiguration> databaseConfiguration, ILogger<BinLogDeleteVisitor> logger) 
            : base(databaseConfiguration, logger)
        {
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is DeleteRowsEvent;
        }

        public ValueTask Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var deleteRows = binlogEvent.Event as DeleteRowsEvent;
            Throw.IfNull(deleteRows, nameof(deleteRows));

            HandleDeleteRowsEvent(deleteRows);

            return new ValueTask(Task.CompletedTask);
        }

        private void HandleDeleteRowsEvent(DeleteRowsEvent deleteRows)
        {
            Console.WriteLine($"{deleteRows.Rows.Count} rows were deleted");
            var eventString = this.GetBinLogEventJson(deleteRows);
           
            foreach (var _ in deleteRows.Rows)
            {
                // Do something
            }

            using (Logger.BeginScope("DeleteRowEvent"))
            {
                Logger.LogInformation(eventString);
                Logger.LogInformation($"{deleteRows.Rows.Count} rows were deleted");
            }
        }
    }
}
