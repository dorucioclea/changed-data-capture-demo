using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ArgumentValidator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Elasticsearch.Abstractions;
using Syncer.Entities;

namespace Syncer.Services.Visitors
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class BinLogCreateVisitor : BaseVisitor<BinLogCreateVisitor>, IBinLogEventVisitor
    {
        public BinLogCreateVisitor(
            IOptions<DatabaseConfiguration> databaseConfiguration, 
            ILogger<BinLogCreateVisitor> logger) 
            : base(databaseConfiguration, logger)
        {
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is WriteRowsEvent;
        }

        public ValueTask Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var writeRows = binlogEvent.Event as WriteRowsEvent;

            Throw.IfNull(writeRows, nameof(writeRows));

            var preProcessInformation = PreProcess(writeRows.TableId, executionContext);

            HandleWriteRowsEvent(writeRows, preProcessInformation);

            return new ValueTask(Task.CompletedTask);
        }
            
        private void HandleWriteRowsEvent(WriteRowsEvent writeRows, PreProcessInformation preProcessInformation)
        {
            var eventString = this.GetBinLogEventJson(writeRows);

            foreach (var _ in writeRows.Rows)
            {
                // Do something
            }

            using (Logger.BeginScope("CreateRowEvent"))
            {
                Logger.LogInformation(eventString);
                Logger.LogInformation($"{writeRows.Rows.Count} rows were written");
            }
        }
    }
}
