using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Database;
using Syncer.Elasticsearch.Documents;

namespace Syncer.Services.Handlers
{
    public abstract class HandlerBase
    {
        protected List<T> GetItemsFrom<T>(IReadOnlyList<ColumnData> rows, List<ColumnConfiguration> columns)
            where T : BaseDocument, new()
        {
            var items = new List<T>();
            foreach (var row in rows)
            {
                var newRow = new T();

                for (var i = 0; i < row.Cells.Count; i++)
                {
                    var rowCell = row.Cells[i];

                    var rowPropertyName = columns.SingleOrDefault(
                        column => column.Id == i);

                    if (rowPropertyName == null)
                    {
                        throw new InvalidOperationException($"Could not process create for type {nameof(T)}");
                    }

                    var prop = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(p => p.GetCustomAttributes(typeof(MapToTableFieldAttribute), false).FirstOrDefault() is MapToTableFieldAttribute mapToField && mapToField.Name == rowPropertyName.Name);

                    prop?.SetValue(newRow, rowCell, null);
                }

                items.Add(newRow);
            }

            return items;
        }
        
    }
}