using System.Threading.Tasks;

namespace Syncer.Contracts
{
    public interface IBinLogSyncService
    {
        void Initialize();
        ValueTask Sync();
    }
}
