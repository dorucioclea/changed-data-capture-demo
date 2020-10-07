using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Services.Handlers
{
    public class TestDocumentUpdateHandler : HandlerBase<TestDocument, int>, IUpdateHandler
    {
        public TestDocumentUpdateHandler(IOptions<ElasticSearchConfiguration> elasticSearchConfiguration) : base(
            elasticSearchConfiguration)
        {

        }

        public async ValueTask HandleUpdate(UpdateRowsEvent updateRows, PreProcessInformation preProcessInformation)
        {
            var beforeUpdateColumnData = updateRows.Rows.Select(row => row.BeforeUpdate).ToList();
            var afterUpdateColumnData = updateRows.Rows.Select(row => row.AfterUpdate).ToList();

            var itemsBeforeUpdate = GetItemsFrom(beforeUpdateColumnData, 
                preProcessInformation.TableConfiguration.Columns);
            
            var itemsAfterUpdate =
                GetItemsFrom(afterUpdateColumnData, preProcessInformation.TableConfiguration.Columns);

            var indexName = GetIndexName();

            var repository = GetElasticRepository();

            // update items

        }
    }
}