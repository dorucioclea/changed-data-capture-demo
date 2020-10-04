using Nest;

namespace Syncer.Elasticsearch.Queries.Common
{
    public class GetByIdQuery<T> : GetQuery<T> where T : class
    {
        public GetByIdQuery(DocumentPath<T> documentPath) : base(documentPath)
        {
            
        }

        protected override GetDescriptor<T> BuildQuery(GetDescriptor<T> descriptor)
        {
            return descriptor;
        }
    }
}