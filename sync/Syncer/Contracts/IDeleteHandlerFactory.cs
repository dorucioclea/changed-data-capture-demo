namespace Syncer.Contracts
{
    public interface IDeleteHandlerFactory
    {
        IDeleteHandler GetDeleteHandler(string tableName);
    }
}