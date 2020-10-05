using System.Linq;
using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Contracts;
using Syncer.Elasticsearch.Abstractions;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Services.Handlers
{
    public class TestDocumentCreateHandler : HandlerBase, ICreateHandler<TestDocument>
    {
        private readonly IElasticsearchRepository _elasticsearchRepository;

        public TestDocumentCreateHandler(IElasticsearchRepository elasticsearchRepository)
        {
            _elasticsearchRepository = elasticsearchRepository;
        }

        public async ValueTask HandleCreate(WriteRowsEvent writeRows, PreProcessInformation preProcessInformation)
        {
            var newItems = GetItemsFrom<TestDocument>(writeRows.Rows, preProcessInformation.TableConfiguration.Columns);
            var indexName = newItems.First().IndexName;
            
            await _elasticsearchRepository.BulkAsync(newItems, indexName, true);
        }
    }
}
