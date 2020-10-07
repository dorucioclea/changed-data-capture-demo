
namespace Syncer.Contracts
{
    public interface ICreateHandlerFactory
    {
        ICreateHandler GetCreateHandler(string tableName);
    }
}
