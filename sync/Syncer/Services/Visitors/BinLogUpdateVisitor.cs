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
    public class BinLogUpdateVisitor : BaseVisitor<BinLogUpdateVisitor>, IBinLogEventVisitor
    {
        public BinLogUpdateVisitor(IOptions<DatabaseConfiguration> databaseConfiguration, ILogger<BinLogUpdateVisitor> logger)
            : base(databaseConfiguration, logger)
        {
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is UpdateRowsEvent;
        }

        public ValueTask Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var updatedRows = binlogEvent.Event as UpdateRowsEvent;
            Throw.IfNull(updatedRows, nameof(updatedRows));

            HandleUpdateRowsEvent(updatedRows);

            return new ValueTask(Task.CompletedTask);
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

            using (Logger.BeginScope("UpdateRowEvent"))
            {
                Logger.LogInformation(eventString);
                Logger.LogInformation($"{updatedRows.Rows.Count} rows were updated");
            }
        }
    }
}
