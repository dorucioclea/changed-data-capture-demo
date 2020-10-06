using System;
using System.Collections.Generic;
using System.Linq;
using Syncer.Contracts;

namespace Syncer.Services.Handlers
{
    public class CreateHandlerFactory : ICreateHandlerFactory
    {
        private readonly List<ICreateHandler> _createHandlers;

        public CreateHandlerFactory(List<ICreateHandler> createHandlers)
        {
            _createHandlers = createHandlers;
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
    }
}
