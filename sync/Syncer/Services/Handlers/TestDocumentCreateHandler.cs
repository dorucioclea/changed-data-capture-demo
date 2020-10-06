using System;
using System.Linq;
using System.Threading.Tasks;
using MySqlCdc.Events;
using Syncer.Contracts;
using Syncer.Elasticsearch.Abstractions;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Services.Handlers
{
    public class TestDocumentCreateHandler : HandlerBase<TestDocument, int>, ICreateHandler
    {
        private readonly IElasticsearchRepository _elasticsearchRepository;

        public TestDocumentCreateHandler(IElasticsearchRepository elasticsearchRepository)
        {
            _elasticsearchRepository = elasticsearchRepository;
        }

        public async ValueTask HandleCreate(WriteRowsEvent writeRows, PreProcessInformation preProcessInformation)
        {
            try
            {
                var newItems = GetItemsFrom(writeRows.Rows, preProcessInformation.TableConfiguration.Columns);
                var indexName = newItems.First().IndexName;

                await _elasticsearchRepository.BulkAsync(newItems, indexName, true);
            }
            catch (Exception exception)
            {

            }
        }

        
    }
}
