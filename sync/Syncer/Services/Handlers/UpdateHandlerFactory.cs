using System;
using System.Collections.Generic;
using System.Linq;
using Syncer.Contracts;

namespace Syncer.Services.Handlers
{
    public class UpdateHandlerFactory : IUpdateHandlerFactory 
    {
        private readonly IEnumerable<IUpdateHandler> _updateHandlers;
        public UpdateHandlerFactory(IEnumerable<IUpdateHandler> updateHandlers)
        {
            _updateHandlers = updateHandlers;
        }

        public IUpdateHandler GetUpdateHandler(string tableName)
        {
            var handler = _updateHandlers.SingleOrDefault(handlerItem => handlerItem.HandledTableName == tableName);

            if (handler == null)
            {
                throw new NullReferenceException($"Could not get an update handler for {tableName}");
            }

            return handler;
        }
    }
}