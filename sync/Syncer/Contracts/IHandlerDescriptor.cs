using System.Linq;
using Syncer.Database;
using Syncer.Elasticsearch.Documents;

namespace Syncer.Contracts
{
    public interface IHandlerDescriptor<T> where T : BaseDocument, new()
    {
        string Name() =>
            typeof(T).GetCustomAttributes(typeof(MapToTableAttribute), true).FirstOrDefault() is MapToTableAttribute mapToTableAttribute
                ? mapToTableAttribute.Name
                : null;
    }
}