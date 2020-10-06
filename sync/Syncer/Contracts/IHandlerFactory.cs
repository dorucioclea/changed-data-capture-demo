
namespace Syncer.Contracts
{
    public interface ICreateHandlerFactory
    {
        ICreateHandler GetCreateHandler(string tableName);
    }

    public interface IDeleteHandlerFactory
    {
        IDeleteHandler GetDeleteHandler(string tableName);
    }
}
