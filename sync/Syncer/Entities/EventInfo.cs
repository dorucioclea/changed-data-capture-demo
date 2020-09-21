using MySqlCdc;
using MySqlCdc.Events;

namespace Syncer.Entities
{
    public class EventInfo
    {
        public BinlogOptions Options { get; set; }

        public IBinlogEvent Event { get; set; }
    }
}
