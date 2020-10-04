using Syncer.Database;

namespace Syncer.Elasticsearch.Documents
{
    [MapToTable("test")]
    public class TestDocument : BaseDocument
    {
        [MapToTableField("name")]
        public string Name { get; set; }

        [MapToTableField("id")]
        public override string Id { get; set; }
    }
}
