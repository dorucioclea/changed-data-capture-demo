using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Services.Visitors;

namespace Syncer.Contracts
{
    public interface IUpdateHandler : IHandler
    {
        ValueTask HandleUpdate(UpdateRowsEvent updateRows, PreProcessInformation preProcessInformation);
    }
}