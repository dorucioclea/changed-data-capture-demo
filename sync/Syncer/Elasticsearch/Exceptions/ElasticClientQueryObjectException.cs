using System;

namespace Syncer.Elasticsearch.Exceptions
{
    public class ElasticClientQueryObjectException : ElasticsearchException
    {
        public ElasticClientQueryObjectException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}