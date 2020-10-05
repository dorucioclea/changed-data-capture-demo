using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Contracts
{
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public interface IDeleteHandler<T> where T: BaseDocument, new()
    {
        ValueTask HandleDelete(DeleteRowsEvent deleteRows, PreProcessInformation preProcessInformation);
    }
}