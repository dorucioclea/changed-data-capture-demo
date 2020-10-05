using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Contracts
{
    public interface ICreateHandler
    {
        ValueTask HandleCreate(WriteRowsEvent writeRows, PreProcessInformation preProcessInformation);
    }

    public interface ICreateHandler<T>: IHandlerDescriptor<T>, ICreateHandler where T: BaseDocument, new()
    {
        
    }
}