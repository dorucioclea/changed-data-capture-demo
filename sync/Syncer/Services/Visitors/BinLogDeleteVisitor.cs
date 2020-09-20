﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;

namespace Syncer.Services.Visitors
{
    public class BinLogDeleteVisitor : IBinLogEventVisitor
    {
        private readonly ILogger<BinLogDeleteVisitor> _logger;

        public BinLogDeleteVisitor(ILogger<BinLogDeleteVisitor> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is DeleteRowsEvent;
        }

        public Task Handle(IBinlogEvent binlogEvent, ExecutionContext executionContext)
        {
            var deleteRows = binlogEvent as DeleteRowsEvent;
            Debug.Assert(deleteRows != null, nameof(deleteRows) + " != null");

            HandleDeleteRowsEvent(deleteRows);

            return Task.CompletedTask;
        }

        private void HandleDeleteRowsEvent(DeleteRowsEvent deleteRows)
        {
            Console.WriteLine($"{deleteRows.Rows.Count} rows were deleted");
            var eventString = this.GetBinLogEventJson(deleteRows);
           
            foreach (var _ in deleteRows.Rows)
            {
                // Do something
            }

            using (_logger.BeginScope("DeleteRowEvent"))
            {
                _logger.LogInformation(eventString);
                _logger.LogInformation($"{deleteRows.Rows.Count} rows were deleted");
            }
        }
    }
}
