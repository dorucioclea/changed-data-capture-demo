namespace Syncer.Entities
{
    public class SyncStatus
    {
        public bool Success { get; set; }
        public bool HasMessages { get; set; }

        public static SyncStatus NoMessages()
        {
            return new SyncStatus
            {
                HasMessages = false,
                Success = false
            };
        }
    }
}
