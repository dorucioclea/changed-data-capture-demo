using System;
using System.Collections.Generic;
using System.Linq;
using Syncer.Contracts;

namespace Syncer.Services.Handlers
{
    public class CreateHandlerFactory : ICreateHandlerFactory
    {
        private readonly IEnumerable<ICreateHandler> _createHandlers;

        public CreateHandlerFactory(IEnumerable<ICreateHandler> createHandlers)
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
