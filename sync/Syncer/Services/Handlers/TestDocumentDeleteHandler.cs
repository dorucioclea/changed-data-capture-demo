using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Services.Handlers
{
    public class TestDocumentDeleteHandler: HandlerBase<TestDocument, int>, IDeleteHandler
    {
        private readonly IOptions<ElasticSearchConfiguration> _elasticSearchConfiguration;

        public TestDocumentDeleteHandler(IOptions<ElasticSearchConfiguration> elasticSearchConfiguration)
        {
            _elasticSearchConfiguration = elasticSearchConfiguration;
        }

        public async ValueTask HandleDelete(DeleteRowsEvent deleteRows, PreProcessInformation preProcessInformation)
        {
            var itemsToDelete = GetItemsFrom(deleteRows.Rows, preProcessInformation.TableConfiguration.Columns);

            var indexName = GetIndexName();

            var repository = GetRepository(_elasticSearchConfiguration.Value);

            foreach (var item in itemsToDelete)
            {
                await repository.DeleteAsync<TestDocument>(item.Id.ToString(), indexName, true);
            }
        }
    }
}