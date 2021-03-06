using System.Threading.Tasks;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;

namespace Syncer.Elasticsearch.Queries
{
	public class DocumentExistsQuery<T> : ElasticClientQueryObject<ExistsResponse> where T : class
	{
		private readonly T _document;

		public DocumentExistsQuery(T document)
		{
			_document = document;
		}

		protected override ExistsResponse ExecuteCore(IElasticClient client, string index)
		{
		    return client.DocumentExists(DocumentPath<T>.Id(_document), desc => BuildQueryCore(desc).Index(index));
        }

	    protected override Task<ExistsResponse> ExecuteCoreAsync(IElasticClient client, string index)
	    {
	        return client.DocumentExistsAsync(DocumentPath<T>.Id(_document), desc => BuildQueryCore(desc).Index(index));
        }

        protected virtual DocumentExistsDescriptor<T> BuildQueryCore(DocumentExistsDescriptor<T> descriptor)
		{
			return BuildQuery(descriptor);
		}

		protected virtual DocumentExistsDescriptor<T> BuildQuery(DocumentExistsDescriptor<T> descriptor)
		{
			return descriptor;
		}
	}
}