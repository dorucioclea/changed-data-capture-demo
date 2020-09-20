using System;
using System.Collections.Generic;
using System.Text;

namespace Syncer.Configuration
{
    public class DatabaseOrdinalConfiguration
    {
        public List<TableConfiguration> Tables { get; set; }
    }

    public class TableConfiguration
    {
        public string DatabaseName { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public List<ColumnConfiguration> Columns { get; set; }
    }

    public class ColumnConfiguration
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
