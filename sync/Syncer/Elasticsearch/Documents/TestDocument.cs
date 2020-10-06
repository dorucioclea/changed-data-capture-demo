using Syncer.Database;

namespace Syncer.Elasticsearch.Documents
{
    [MapToTable("test")]
    public class TestDocument : BaseDocument<int>
    {
        [MapToTableField("name")]
        public string Name { get; set; }

        [MapToTableField("id")]
        public override int Id { get; set; }
    }
}
