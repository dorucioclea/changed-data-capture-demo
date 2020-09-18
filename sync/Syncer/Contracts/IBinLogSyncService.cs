using System.Threading.Tasks;
using Syncer.Entities;

namespace Syncer.Contracts
{
    public interface IBinLogSyncService
    {
        void Initialize();
        ValueTask<SyncStatus> Sync();
    }
}
