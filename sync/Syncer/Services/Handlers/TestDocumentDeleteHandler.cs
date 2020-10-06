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
        public TestDocumentDeleteHandler(IOptions<ElasticSearchConfiguration> elasticSearchConfiguration) : base(elasticSearchConfiguration)
        {
        }

        public async ValueTask HandleDelete(DeleteRowsEvent deleteRows, PreProcessInformation preProcessInformation)
        {
            var itemsToDelete = GetItemsFrom(deleteRows.Rows, preProcessInformation.TableConfiguration.Columns);

            var indexName = GetIndexName();

            var repository = GetElasticRepository();

            foreach (var item in itemsToDelete)
            {
                await repository.DeleteAsync<TestDocument>(item.Id.ToString(), indexName, true);
            }
        }
    }
}