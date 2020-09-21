using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Entities;

namespace Syncer.Services.Visitors
{
    public class BinLogPositionVisitor : IBinLogEventVisitor
    {
        private readonly ILogger<BinLogPositionVisitor> _logger;
        private readonly IOptions<BinLogConfiguration> _binLogConfiguration;

        public BinLogPositionVisitor(ILogger<BinLogPositionVisitor> logger, IOptions<BinLogConfiguration> binLogConfiguration)
        {
            _logger = logger;
            _binLogConfiguration = binLogConfiguration;
        }

        public bool CanHandle(IBinlogEvent binLogEvent)
        {
            return _binLogConfiguration.Value.AutoSave;
        }

        public async ValueTask Handle(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            if (binlogEvent.Options.Filename != null)
            {
                await HandlePosition(binlogEvent, executionContext);
            }
        }

        private async ValueTask HandlePosition(EventInfo binlogEvent, ExecutionContext executionContext)
        {
            var binLog = new BinLog(binlogEvent.Options.Filename, binlogEvent.Options.Position);

            await File.WriteAllTextAsync(executionContext.BinLogLedgerPath,
                $"{binLog}");

            _logger.LogDebug($"Logged binlog position { binLog }");

            executionContext.BinLogFilename = binlogEvent.Options.Filename;
            executionContext.BinLogPosition = binlogEvent.Options.Position;
        }
    }
}
