using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Syncer.Contracts;
using Syncer.Database;
using Syncer.Elasticsearch.Documents;

namespace Syncer.Services.Handlers
{
    public class HandlerFence : IHandlerFence
    {
        private readonly List<string> _tableNames;

        public HandlerFence()
        {
            _tableNames = GetEnumerableOfType<BaseDocument>().Select(GetHandledTable).ToList();
        }

        public bool CanHandleTable(string tableName)
        {
            return _tableNames.Contains(tableName);
        }

        private static string GetHandledTable(MemberInfo documentType)
        {
            var mapToTableAttributes = documentType.GetCustomAttributes(typeof(MapToTableAttribute)).ToList();

            if (!mapToTableAttributes.Any()) return null;
            var firstAttribute = mapToTableAttributes.FirstOrDefault();

            if (!(firstAttribute is MapToTableAttribute mapToTableAttribute)) return null;
            var name = mapToTableAttribute.Name;

            return name;
        }

        private static IEnumerable<Type> GetEnumerableOfType<T>() where T : class
        {
            var thisAssembly = Assembly.GetAssembly(typeof(HandlerFence));

            if (thisAssembly == null)
            {
                throw new ApplicationException("Could not get the current assembly");
            }

            var objects = thisAssembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
                .ToList();
            
            return objects;
        }
    }
}