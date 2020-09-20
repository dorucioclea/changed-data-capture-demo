using MySqlCdc;
using Syncer.Configuration;

namespace Syncer.Contracts
{
    public interface ICdcClientProvider
    {
        BinlogClient Provide(DatabaseConfiguration databaseConfiguration, BinLogConfiguration binLogConfiguration);
    }
}
