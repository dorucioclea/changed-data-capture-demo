using Nest;

namespace Syncer.Elasticsearch.Documents
{
    [ElasticsearchType(IdProperty = "Id", RelationName = "_doc")]
    public abstract class BaseDocument<T> where T : struct
    {
        [Keyword]
        public abstract T Id { get; set; }

        [Keyword(Name = "_docType")]
        public string DocType { get; set; }

        protected BaseDocument()
        {
            DocType = $"{GetType().FullName}, {GetType().Assembly.GetName().Name}";
        }
    }
}
