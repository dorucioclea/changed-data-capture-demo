using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;

namespace Syncer.Services.Visitors
{
    public class BinLogCreateVisitor : IBinLogEventVisitor
    {
        private readonly ILogger<BinLogCreateVisitor> _logger;

        public BinLogCreateVisitor(ILogger<BinLogCreateVisitor> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is WriteRowsEvent;
        }

        public Task Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var writeRows = binlogEvent.Event as WriteRowsEvent;

            Debug.Assert(writeRows != null, nameof(writeRows) + " != null");

            HandleWriteRowsEvent(writeRows);

            return Task.CompletedTask;
        }

        private void HandleWriteRowsEvent(WriteRowsEvent writeRows)
        {
            var eventString = this.GetBinLogEventJson(writeRows);

            foreach (var _ in writeRows.Rows)
            {
                // Do something
            }

            using (_logger.BeginScope("CreateRowEvent"))
            {
                _logger.LogInformation(eventString);
                _logger.LogInformation($"{writeRows.Rows.Count} rows were written");
            }
        }
    }
}
