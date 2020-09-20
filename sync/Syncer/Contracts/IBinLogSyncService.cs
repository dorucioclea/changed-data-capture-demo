using System.Threading;
using System.Threading.Tasks;

namespace Syncer.Contracts
{
    public interface IBinLogSyncService
    {
        ValueTask Sync(CancellationToken cancellationToken);
    }
}
