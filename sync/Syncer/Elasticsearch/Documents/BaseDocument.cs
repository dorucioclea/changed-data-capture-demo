using Nest;

namespace Syncer.Elasticsearch.Documents
{
    [ElasticsearchType(IdProperty = "Id", Name = "_doc")]
    public abstract class BaseDocument
    {
        [Keyword]
        public abstract string Id { get; set; }

        [Keyword(Name = "_docType")]
        public string DocType { get; set; }

        protected BaseDocument()
        {
            DocType = $"{GetType().FullName}, {GetType().Assembly.GetName().Name}";
        }
    }
}
