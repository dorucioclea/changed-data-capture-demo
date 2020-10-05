using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Services.Visitors;

namespace Syncer.Contracts
{
    public interface IDeleteHandler
    {
        ValueTask HandleDelete(DeleteRowsEvent deleteRows, PreProcessInformation preProcessInformation);
    }
}