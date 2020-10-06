using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Services.Visitors;

namespace Syncer.Contracts
{
    public interface ICreateHandler : IHandler
    {
        ValueTask HandleCreate(WriteRowsEvent writeRows, PreProcessInformation preProcessInformation);
    }
}