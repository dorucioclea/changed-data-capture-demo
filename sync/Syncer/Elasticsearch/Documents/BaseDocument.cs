using Nest;

namespace Syncer.Elasticsearch.Documents
{
    [ElasticsearchType(IdProperty = "Id", RelationName = "_doc")]
    public abstract class BaseDocument
    {
        [Keyword]
        public abstract string Id { get; set; }

        [Keyword(Name = "_docType")]
        public string DocType { get; set; }

        [Text(Ignore = true)] public string IndexName => nameof(GetType);

        protected BaseDocument()
        {
            DocType = $"{GetType().FullName}, {GetType().Assembly.GetName().Name}";
        }
    }
}
