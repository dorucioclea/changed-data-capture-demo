using System.Threading.Tasks;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;

namespace Syncer.Elasticsearch.Queries
{
    public abstract class UpdateWithQueryObject<T> : ElasticClientQueryObject<UpdateByQueryResponse> where T : class
    {
        protected override UpdateByQueryResponse ExecuteCore(IElasticClient client, string index)
        {
            return client.UpdateByQuery<T>(desc => BuildQuery(desc).Index(index));
        }

        protected override Task<UpdateByQueryResponse> ExecuteCoreAsync(IElasticClient client, string index)
        {
            return client.UpdateByQueryAsync<T>(desc => BuildQuery(desc).Index(index));
        }

        protected abstract UpdateByQueryDescriptor<T> BuildQuery(UpdateByQueryDescriptor<T> descriptor);
    }
}