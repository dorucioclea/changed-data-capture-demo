using System.Collections.Generic;

namespace Syncer.Configuration
{
    public class ExecutionContext
    {
        public DatabaseOrdinalConfiguration OrdinalConfiguration { get; set; }

        public ExecutionContext()
        {
            OrdinalConfiguration = new DatabaseOrdinalConfiguration
            {
                Tables = new List<TableConfiguration>()
            };
        }
    }
}
