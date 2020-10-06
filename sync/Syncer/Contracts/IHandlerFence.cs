namespace Syncer.Contracts
{
    public interface IHandlerFence
    {
        bool CanHandleTable(string tableName);
    }
}