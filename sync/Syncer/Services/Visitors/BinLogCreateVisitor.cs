using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;
using ArgumentValidator;
using Microsoft.Extensions.Options;
using Syncer.Entities;

namespace Syncer.Services.Visitors
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class BinLogCreateVisitor : BaseVisitor<BinLogCreateVisitor>, IBinLogEventVisitor
    {
        private readonly ILogger<BinLogCreateVisitor> _logger;

        public BinLogCreateVisitor(IOptions<DatabaseConfiguration> databaseConfiguration, ILogger<BinLogCreateVisitor> logger) : base(databaseConfiguration, logger)
        {
            _logger = logger;
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

            using (_logger.BeginScope("CreateRowEvent"))
            {
                _logger.LogInformation(eventString);
                _logger.LogInformation($"{writeRows.Rows.Count} rows were written");
            }
        }
    }
}
