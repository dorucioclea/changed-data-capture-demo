using Nest;

namespace Syncer.Elasticsearch.Extensions
{
    internal static class ElasticClientExtensions
    {
        internal static string GetDefaultIndex(this IElasticClient client)
        {
            return client.ConnectionSettings.DefaultIndex;
        }
    }
}