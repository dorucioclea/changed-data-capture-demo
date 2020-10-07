using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;
using Syncer.Elasticsearch.Documents;

namespace Syncer.Elasticsearch.Queries.Common
{
    public class UpdateByIdQuery<T> : ElasticClientQueryObject<UpdateResponse<T>> where T : class
    {
        private readonly string _id;
        private readonly T _newObject;
        private readonly bool _refreshOnDelete;

        public UpdateByIdQuery(string id, T newObject, bool refreshOnDelete = false)
        {
            _id = id;
            _newObject = newObject;
            _refreshOnDelete = refreshOnDelete;
        }

        protected override UpdateResponse<T> ExecuteCore(IElasticClient client, string index)
        {
            var document = DocumentPath<T>.Id(_id);

            return client.Update(document, descriptor => BuildQueryCore(descriptor).Index(index));
        }

        protected override Task<UpdateResponse<T>> ExecuteCoreAsync(IElasticClient client, string index)
        {
            var document = DocumentPath<T>.Id(_id);
            return client.UpdateAsync(document, descriptor => BuildQueryCore(descriptor).Index(index));
        }

        protected virtual UpdateDescriptor<T, T> BuildQueryCore(UpdateDescriptor<T,T> descriptor)
        {
            Refresh refresh = _refreshOnDelete ? Refresh.True : Refresh.False;

            descriptor = descriptor.Doc(_newObject).RetryOnConflict(3).Refresh(refresh);
            return BuildQuery(descriptor);
        }

        protected virtual UpdateDescriptor<T, T> BuildQuery(UpdateDescriptor<T, T> descriptor)
        {
            return descriptor;
        }
    }
}