using System;
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
    public class BinLogDeleteVisitor : BaseVisitor<BinLogDeleteVisitor>, IBinLogEventVisitor
    {
        private readonly IDeleteHandlerFactory _deleteHandlerFactory;

        public BinLogDeleteVisitor(
            IOptions<DatabaseConfiguration> databaseConfiguration, 
            IDeleteHandlerFactory deleteHandlerFactory,
            ILogger<BinLogDeleteVisitor> logger) 
            : base(databaseConfiguration, logger)
        {
            _deleteHandlerFactory = deleteHandlerFactory;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return binLogEvent is DeleteRowsEvent;
        }

        public async ValueTask Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var deleteRows = binlogEvent.Event as DeleteRowsEvent;
            Throw.IfNull(deleteRows, nameof(deleteRows));

            var preProcessInformation = PreProcess(deleteRows.TableId, executionContext);

            await HandleDeleteRowsEvent(deleteRows, preProcessInformation);
        }

        private async ValueTask HandleDeleteRowsEvent(DeleteRowsEvent deleteRows, PreProcessInformation preProcessInformation)
        {
            Console.WriteLine($"{deleteRows.Rows.Count} rows were deleted");
            var eventString = this.GetBinLogEventJson(deleteRows);

            var handler = _deleteHandlerFactory.GetDeleteHandler(preProcessInformation.TableConfiguration.Name);

            await handler.HandleDelete(deleteRows, preProcessInformation);

            using (Logger.BeginScope("DeleteRowEvent"))
            {
                Logger.LogInformation(eventString);
                Logger.LogInformation($"{deleteRows.Rows.Count} rows were deleted");
            }
        }
    }
}
