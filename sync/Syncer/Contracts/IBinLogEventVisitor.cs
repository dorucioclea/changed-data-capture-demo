using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Configuration;

namespace Syncer.Contracts
{
    public interface IBinLogEventVisitor
    {
        public bool CanHandle(IBinlogEvent binLogEvent);

        public Task Handle(IBinlogEvent binlogEvent, ExecutionContext executionContext);
    }
}
