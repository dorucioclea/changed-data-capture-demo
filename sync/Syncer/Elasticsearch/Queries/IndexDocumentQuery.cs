using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;

namespace Syncer.Elasticsearch.Queries
{
	public class IndexDocumentQuery<T> : ElasticClientQueryObject<IndexResponse> where T : class
	{
		private readonly T _document;
		private readonly bool _refreshOnSave;

		public IndexDocumentQuery(T document, bool refreshOnSave = false)
		{
			_document = document;
			_refreshOnSave = refreshOnSave;
		}

		protected override IndexResponse ExecuteCore(IElasticClient client, string index)
		{
			return client.Index(_document, desc => BuildQueryCore(desc, _refreshOnSave).Index(index));
		}

	    protected override Task<IndexResponse> ExecuteCoreAsync(IElasticClient client, string index)
	    {
            return client.IndexAsync(_document, desc => BuildQueryCore(desc, _refreshOnSave).Index(index));
        }

	    protected virtual IndexDescriptor<T> BuildQueryCore(IndexDescriptor<T> descriptor, bool refreshOnSave)
        {
            var elasticRefresh = refreshOnSave ? Refresh.True : Refresh.False;

			descriptor = descriptor
				.Refresh(elasticRefresh);
			return BuildQuery(descriptor);
		}
        
		protected virtual IndexDescriptor<T> BuildQuery(IndexDescriptor<T> descriptor)
		{
			return descriptor;
		}
	}
}