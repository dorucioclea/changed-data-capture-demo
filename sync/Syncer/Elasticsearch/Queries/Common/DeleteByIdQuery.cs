using System.Threading.Tasks;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;
using Syncer.Elasticsearch.Extensions;

namespace Syncer.Elasticsearch.Queries.Common
{
    public class DeleteByIdQuery<T> : ElasticClientQueryObject<DeleteResponse> where T : class
    {
        private readonly string _id;
        private readonly bool _refreshOnDelete;

        public DeleteByIdQuery(string id, bool refreshOnDelete = false)
        {
            _id = id;
            _refreshOnDelete = refreshOnDelete;
        }

        protected override DeleteResponse ExecuteCore(IElasticClient client, string index)
        {
            var id = DocumentPath<T>.Id(_id);
            return client.Delete(id ,descriptor => BuildQueryCore(descriptor).Index(index));
        }

        protected override Task<DeleteResponse> ExecuteCoreAsync(IElasticClient client, string index)
        {
            var id = DocumentPath<T>.Id(_id);
            return client.DeleteAsync(id, descriptor => BuildQueryCore(descriptor).Index(index));
        }

        protected virtual DeleteDescriptor<T> BuildQueryCore(DeleteDescriptor<T> descriptor)
        {
            descriptor = descriptor
                .Refresh(_refreshOnDelete);
            return BuildQuery(descriptor);
        }

        protected virtual DeleteDescriptor<T> BuildQuery(DeleteDescriptor<T> descriptor)
        {
            return descriptor;
        }
    }
}