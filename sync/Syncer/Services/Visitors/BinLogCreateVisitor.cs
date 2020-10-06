using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class BinLogCreateVisitor : BaseVisitor<BinLogCreateVisitor>, IBinLogEventVisitor
    {
        private readonly ICreateHandlerFactory _createHandlerFactory;

        public BinLogCreateVisitor(
            IOptions<DatabaseConfiguration> databaseConfiguration,
            ICreateHandlerFactory createHandlerFactory,
            ILogger<BinLogCreateVisitor> logger) 
            : base(databaseConfiguration, logger)
        {
            _createHandlerFactory = createHandlerFactory;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is WriteRowsEvent;
        }

        public async ValueTask Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var writeRows = binlogEvent.Event as WriteRowsEvent;

            Throw.IfNull(writeRows, nameof(writeRows));

            var preProcessInformation = PreProcess(writeRows.TableId, executionContext);

            await HandleWriteRowsEvent(writeRows, preProcessInformation);
        }
            
        private async ValueTask HandleWriteRowsEvent(WriteRowsEvent writeRows, PreProcessInformation preProcessInformation)
        {
            var eventString = this.GetBinLogEventJson(writeRows);

            var handler = _createHandlerFactory.GetCreateHandler(preProcessInformation.TableConfiguration.Name);

            await handler.HandleCreate(writeRows, preProcessInformation);

            using (Logger.BeginScope("CreateRowEvent"))
            {
                Logger.LogInformation(eventString);
                Logger.LogInformation($"{writeRows.Rows.Count} rows were written");
            }
        }
    }
}
