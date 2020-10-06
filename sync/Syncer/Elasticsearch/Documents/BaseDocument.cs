using Nest;

namespace Syncer.Elasticsearch.Documents
{
    public abstract class BaseDocument
    {
        [Keyword(Name = "_docType")]
        public string DocType { get; set; }

        protected BaseDocument()
        {
            DocType = $"{GetType().FullName}, {GetType().Assembly.GetName().Name}";
        }
    }

    [ElasticsearchType(IdProperty = "Id", RelationName = "_doc")]
    public abstract class BaseDocument<T>: BaseDocument where T : struct
    {
        [Keyword]
        public abstract T Id { get; set; }
    }
}
