using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Contracts
{
    public interface IDeleteHandler
    {
        ValueTask HandleDelete(DeleteRowsEvent deleteRows, PreProcessInformation preProcessInformation);
    }

    
    public interface IDeleteHandler<T> : IHandlerDescriptor<T>, IDeleteHandler where T : BaseDocument, new()
    {

    }
}