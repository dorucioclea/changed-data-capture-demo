using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;

namespace Syncer.Elasticsearch.Queries
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class DeleteWithQueryObject<T> : ElasticClientQueryObject<DeleteByQueryResponse> where T : class
    {
        protected override DeleteByQueryResponse ExecuteCore(IElasticClient client, string index)
        {
            return client.DeleteByQuery<T>(desc => BuildQuery(desc).Index(index));
        }

        protected override Task<DeleteByQueryResponse> ExecuteCoreAsync(IElasticClient client, string index)
        {
            return client.DeleteByQueryAsync<T>(desc => BuildQuery(desc).Index(index));
        }

        protected abstract DeleteByQueryDescriptor<T> BuildQuery(DeleteByQueryDescriptor<T> descriptor);
    }
}