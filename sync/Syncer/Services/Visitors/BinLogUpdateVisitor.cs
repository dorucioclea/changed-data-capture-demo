using System.Threading.Tasks;
using ArgumentValidator;
using Microsoft.Extensions.Logging;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;

namespace Syncer.Services.Visitors
{
    public class BinLogUpdateVisitor : IBinLogEventVisitor
    {
        private readonly ILogger<BinLogUpdateVisitor> _logger;

        public BinLogUpdateVisitor(ILogger<BinLogUpdateVisitor> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is UpdateRowsEvent;
        }

        public Task Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var updatedRows = binlogEvent.Event as UpdateRowsEvent;
            Throw.IfNull(updatedRows, nameof(updatedRows));

            HandleUpdateRowsEvent(updatedRows);

            return Task.CompletedTask;
        }

        private void HandleUpdateRowsEvent(UpdateRowsEvent updatedRows)
        {
            var eventString = this.GetBinLogEventJson(updatedRows);

            foreach (var row in updatedRows.Rows)
            {
                var _ = row.BeforeUpdate;
                var __ = row.AfterUpdate;
                // Do something
            }

            using (_logger.BeginScope("UpdateRowEvent"))
            {
                _logger.LogInformation(eventString);
                _logger.LogInformation($"{updatedRows.Rows.Count} rows were updated");
            }
        }
    }
}
