using System;
using System.Threading.Tasks;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;

namespace Syncer.Elasticsearch.Queries
{
    public abstract class GetQuery<T> : ElasticClientQueryObject<IGetResponse<T>> where T : class
    {
        internal const string ObsoleteMessage = "Use alternate constructor accepting DocumentPath";
        private readonly DocumentPath<T> _documentPath;

        [Obsolete(ObsoleteMessage)]
        protected GetQuery(string id)
        {
            _documentPath = DocumentPath<T>.Id(id);
        }

        protected GetQuery(DocumentPath<T> documentPath)
        {
            this._documentPath = documentPath;
        }

        protected override IGetResponse<T> ExecuteCore(IElasticClient client, string index)
        {
            return client.Get(_documentPath, desc => BuildQuery(desc).Index(index));
        }

        protected override Task<IGetResponse<T>> ExecuteCoreAsync(IElasticClient client, string index)
        {
            return client.GetAsync(_documentPath, desc => BuildQuery(desc).Index(index));
        }

        protected abstract GetDescriptor<T> BuildQuery(GetDescriptor<T> descriptor);
    }
}