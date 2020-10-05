using System.Linq;
using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Contracts;
using Syncer.Elasticsearch.Abstractions;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Services.Handlers
{
    public class TestDocumentDeleteHandler: HandlerBase, IDeleteHandler<TestDocument>
    {
        private readonly IElasticsearchRepository _elasticsearchRepository;

        public TestDocumentDeleteHandler(IElasticsearchRepository elasticsearchRepository)
        {
            _elasticsearchRepository = elasticsearchRepository;
        }

        public async ValueTask HandleDelete(DeleteRowsEvent deleteRows, PreProcessInformation preProcessInformation)
        {
            var itemsToDelete = GetItemsFrom<TestDocument>(deleteRows.Rows, preProcessInformation.TableConfiguration.Columns);
            var indexName = itemsToDelete.First().IndexName;

            foreach (var item in itemsToDelete)
            {
                await _elasticsearchRepository.DeleteAsync<TestDocument>(item.Id, indexName, true);
            }
        }
    }
}