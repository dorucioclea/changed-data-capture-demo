using System;
using System.Collections.Generic;
using System.Linq;
using Syncer.Contracts;

namespace Syncer.Services.Handlers
{
    public class DeleteHandlerFactory : IDeleteHandlerFactory
    {
        private readonly IEnumerable<IDeleteHandler> _deleteHandlers;
        public DeleteHandlerFactory(IEnumerable<IDeleteHandler> deleteHandlers)
        {
            _deleteHandlers = deleteHandlers;
        }


        public IDeleteHandler GetDeleteHandler(string tableName)
        {
            var handler = _deleteHandlers.SingleOrDefault(handlerItem => handlerItem.HandledTableName == tableName);

            if (handler == null)
            {
                throw new NullReferenceException($"Could not get a delete handler for {tableName}");
            }

            return handler;
        }
    }
}