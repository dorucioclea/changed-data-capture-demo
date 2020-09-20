using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlCdc.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Syncer.Configuration;

namespace Syncer.Contracts
{
    public interface IBinLogEventVisitor
    {
        public bool CanHandle(IBinlogEvent binLogEvent);

        public Task Handle(IBinlogEvent binlogEvent, ExecutionContext executionContext);
    }

    public static class BinLogEventVisitorExtensions
    {
        public static string GetBinLogEventJson(this IBinLogEventVisitor _, IBinlogEvent binlogEvent)
        {
            var json = JsonConvert.SerializeObject(binlogEvent, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Converters = new List<JsonConverter> { new StringEnumConverter() }
                });

            return json;
        }
    }
}
