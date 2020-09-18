using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Syncer.Contracts
{
    public interface IBinLogSyncService
    {
        ValueTask Sync();
    }
}
