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
    public class BinLogUpdateVisitor : BaseVisitor<BinLogUpdateVisitor>, IBinLogEventVisitor
    {
        private readonly IHandlerFence _handlerFence;
        private readonly IUpdateHandlerFactory _updateHandlerFactory;

        public BinLogUpdateVisitor(IOptions<DatabaseConfiguration> databaseConfiguration, ILogger<BinLogUpdateVisitor> logger, IHandlerFence handlerFence, IUpdateHandlerFactory updateHandlerFactory)
            : base(databaseConfiguration, logger)
        {
            _handlerFence = handlerFence;
            _updateHandlerFactory = updateHandlerFactory;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is UpdateRowsEvent;
        }

        public async ValueTask Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var updatedRows = binlogEvent.Event as UpdateRowsEvent;
            Throw.IfNull(updatedRows, nameof(updatedRows));

            var preProcessInformation = PreProcess(updatedRows.TableId, executionContext);

            if (_handlerFence.CanHandleTable(preProcessInformation.TableConfiguration.Name))
            {
                await HandleUpdateRowsEvent(updatedRows, preProcessInformation);
            }
        }

        private async ValueTask HandleUpdateRowsEvent(UpdateRowsEvent updatedRows, PreProcessInformation preProcessInformation)
        {
            var eventString = this.GetBinLogEventJson(updatedRows);

            var handler = _updateHandlerFactory.GetUpdateHandler(preProcessInformation.TableConfiguration.Name);

            await handler.HandleUpdate(updatedRows, preProcessInformation);

            using (Logger.BeginScope("UpdateRowEvent"))
            {
                Logger.LogInformation(eventString);
                Logger.LogInformation($"{updatedRows.Rows.Count} rows were updated");
            }
        }
    }
}
