using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Entities;

namespace Syncer.Services.Visitors
{
    public class BinLogPositionVisitor : IBinLogEventVisitor
    {
        private readonly ILogger<BinLogPositionVisitor> _logger;

        public BinLogPositionVisitor(ILogger<BinLogPositionVisitor> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return true;
        }

        public async Task Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            if (binlogEvent.Options.Filename != null)
            {
                var binLog = new BinLog(binlogEvent.Options.Filename,  binlogEvent.Options.Position);

                await File.WriteAllTextAsync(executionContext.BinLogLedgerPath,
                    $"{binLog}");

                _logger.LogDebug($"Logged binlog position { binLog }");

                executionContext.BinLogFilename = binlogEvent.Options.Filename;
                executionContext.BinLogPosition = binlogEvent.Options.Position;
            }
        }
    }
}
