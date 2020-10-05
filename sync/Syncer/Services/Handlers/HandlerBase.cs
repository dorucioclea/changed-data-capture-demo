using System;
using System.Collections.Generic;
using System.Linq;
using MySqlCdc.Events;
using Syncer.Configuration;
using Syncer.Elasticsearch.Documents;

namespace Syncer.Services.Handlers
{
    public abstract class HandlerBase<T> where T: BaseDocument, new()
    {
        protected List<T> GetItemsFrom(IReadOnlyList<ColumnData> rows, List<ColumnConfiguration> columns)
        {
            var items = new List<T>();
            foreach (var row in rows)
            {
                var newRow = new T();
                var type = newRow.GetType();

                for (var i = 0; i < row.Cells.Count; i++)
                {
                    var rowCell = row.Cells[i];

                    var rowPropertyName = columns.SingleOrDefault(
                        column => column.Id == i);

                    if (rowPropertyName == null)
                    {
                        throw new InvalidOperationException($"Could not process create for type {nameof(T)}");
                    }


                    var prop = type.GetProperty(rowPropertyName.Name);
                    prop?.SetValue(newRow, rowCell, null);
                }

                items.Add(newRow);
            }

            return items;
        }
        
    }
}