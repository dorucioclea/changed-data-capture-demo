using System;
using System.Collections.Generic;
using System.Linq;
using Syncer.Contracts;

namespace Syncer.Services.Handlers
{
    public class HandlerFactory : IHandlerFactory
    {
        private readonly List<ICreateHandler> _createHandlers;
        private readonly List<IDeleteHandler> _deleteHandlers;

        public HandlerFactory(List<ICreateHandler> createHandlers, List<IDeleteHandler> deleteHandlers)
        {
            _createHandlers = createHandlers;
            _deleteHandlers = deleteHandlers;
        }

        public ICreateHandler GetCreateHandler(string tableName)
        {
            var handler = _createHandlers.SingleOrDefault(handlerItem => handlerItem.HandledTableName == tableName);

            if (handler == null)
            {
                throw new NullReferenceException($"Could not get a create handler for {tableName}");
            }

            return handler;
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
