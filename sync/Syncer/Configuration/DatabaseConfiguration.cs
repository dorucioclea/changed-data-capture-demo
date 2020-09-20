namespace Syncer.Configuration
{
    /// <summary>
    ///  "CanalConfiguration" : {
    /// "ServerAddress": "",
    /// "ServerPort": "",
    /// "DatabaseName": "",
    ///    "Credentials": {
    ///    "UserName": "",
    ///    "Password": ""
    ///    }  
    /// } 
    /// </summary>
    public class DatabaseConfiguration
    {
        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public Credentials Credentials { get; set; }
    }
}
