using Nest;

namespace Syncer.Elasticsearch.Abstractions.Queries
{
	public abstract class SearchQueryObject<TDocument> : ElasticClientQueryObject<ISearchResponse<TDocument>> where TDocument : class
	{
	}
}