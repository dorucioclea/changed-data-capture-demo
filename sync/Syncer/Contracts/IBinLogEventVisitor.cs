using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlCdc;
using MySqlCdc.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Syncer.Configuration;
using Syncer.Entities;

namespace Syncer.Contracts
{
    public interface IBinLogEventVisitor
    {
        public bool CanHandle(IBinlogEvent binLogEvent);

        public Task Handle(EventInfo binlogEvent, ExecutionContext executionContext);
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
