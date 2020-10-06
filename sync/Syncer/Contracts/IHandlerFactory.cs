using System;
using System.Collections.Generic;
using System.Text;

namespace Syncer.Contracts
{
    public interface IHandlerFactory
    {
        ICreateHandler GetCreateHandler(string tableName);
        IDeleteHandler GetDeleteHandler(string tableName);
    }
}
