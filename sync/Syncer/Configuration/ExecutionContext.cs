using System.Collections.Generic;

namespace Syncer.Configuration
{
    public class ExecutionContext
    {
        public string BinLogLedgerPath { get; set; }

        public DatabaseOrdinalConfiguration OrdinalConfiguration { get; set; }

        public string BinLogFilename { get; set; }

        public long BinLogPosition { get; set; }

        public ExecutionContext()
        {
            OrdinalConfiguration = new DatabaseOrdinalConfiguration
            {
                Tables = new List<TableConfiguration>()
            };
        }
    }
}
