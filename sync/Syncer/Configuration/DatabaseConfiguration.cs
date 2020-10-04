using System.Collections.Generic;

namespace Syncer.Configuration
{
    public class DatabaseConfiguration
    {
        public string ServerAddress { get; set; }

        public int ServerId { get; set; }

        public int ServerPort { get; set; }

        public string Database { get; set; }

        public Credentials Credentials { get; set; }

        public List<string> HandleTables { get; set; }
    }
}
