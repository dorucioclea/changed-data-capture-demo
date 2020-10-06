using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySqlCdc.Events;
using Nest;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Elasticsearch.Documents;
using Syncer.Services.Visitors;

namespace Syncer.Services.Handlers
{
    public class TestDocumentCreateHandler : HandlerBase<TestDocument, int>, ICreateHandler
    {

        public TestDocumentCreateHandler(IOptions<ElasticSearchConfiguration> elasticSearchConfiguration) : base(elasticSearchConfiguration)
        {
        }

        public async ValueTask HandleCreate(WriteRowsEvent writeRows, PreProcessInformation preProcessInformation)
        {
            var newItems = GetItemsFrom(writeRows.Rows, preProcessInformation.TableConfiguration.Columns);

            var indexName = GetIndexName();

            var repository = GetElasticRepository();

            await repository.BulkAsync(newItems, indexName, true);
        }

        protected override void CreateIndex(IElasticClient client, string indexName)
        {
            client.Indices.Create(indexName, c => c
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(aa => aa
                            .Standard("standard_english", sa => sa
                                .StopWords("_english_")
                            )
                        )
                    )
                )
                .Map<TestDocument>(m => m
                    .AutoMap()
                    .Properties(p => p
                        .Text(t => t
                            // should we exclude 'The', 'and' etc. from ebook title?
                            .Name(x => x.Name)
                            .Analyzer("standard_english")
                        )
                    )
                )
            );
        }
    }
}
