namespace Syncer.Contracts
{
    public interface IUpdateHandlerFactory
    {
        IUpdateHandler GetUpdateHandler(string tableName);
    }
}