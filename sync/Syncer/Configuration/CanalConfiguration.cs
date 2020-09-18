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
    public class CanalConfiguration
    {
        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public string DatabaseName { get; set; }

        public CanalCredentials Credentials { get; set; }
    }

    public class CanalCredentials
    {
        public string UserName { get; set; }
        
        public string Password { get; set; }
    }
}
