using System.Threading.Tasks;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;

namespace Syncer.Elasticsearch.Queries
{
    public abstract class MultiSearchQuery : ElasticClientQueryObject<IMultiSearchResponse>
    {
        protected override IMultiSearchResponse ExecuteCore(IElasticClient client, string index)
        {
			return client.MultiSearch(desc => BuildQueryCore(desc, index));
        }

        protected override Task<IMultiSearchResponse> ExecuteCoreAsync(IElasticClient client, string index)
        {
            return client.MultiSearchAsync(desc => BuildQueryCore(desc, index));
        }

        protected MultiSearchDescriptor BuildQueryCore(MultiSearchDescriptor descriptor, string index)
        {
            descriptor = BuildQuery(descriptor, index);
            return descriptor;
        }

        protected abstract MultiSearchDescriptor BuildQuery(MultiSearchDescriptor descriptor, string index);
    }
}