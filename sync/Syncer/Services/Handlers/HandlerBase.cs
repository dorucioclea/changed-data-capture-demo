using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using MySqlCdc.Events;
using Nest;
using Syncer.Configuration;
using Syncer.Contracts;
using Syncer.Database;
using Syncer.Elasticsearch;
using Syncer.Elasticsearch.Abstractions;
using Syncer.Elasticsearch.Documents;

namespace Syncer.Services.Handlers
{
    public abstract class HandlerBase<T, TK> : IHandler where T: BaseDocument<TK>, new() where TK: struct
    {
        private readonly IOptions<ElasticSearchConfiguration> _elasticSearchConfigurationOptions;

        protected HandlerBase(IOptions<ElasticSearchConfiguration> elasticSearchConfigurationOptions)
        {
            _elasticSearchConfigurationOptions = elasticSearchConfigurationOptions;
        }

        public string HandledTableName => GetHandledTableName();

        protected virtual void CreateIndex(IElasticClient client, string indexName) { }


        protected List<T> GetItemsFrom(IReadOnlyList<ColumnData> rows, List<ColumnConfiguration> columns)
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
        
        protected IElasticsearchRepository GetElasticRepository()
        {
            var elasticSearchConfiguration = _elasticSearchConfigurationOptions.Value;
            var targetIndexName = GetIndexName();

            var connectionSettings = new ConnectionSettings(new Uri(elasticSearchConfiguration.Host))
                .BasicAuthentication(elasticSearchConfiguration.UserName, elasticSearchConfiguration.Password)
                .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
                .DefaultMappingFor<TestDocument>(i => i.IndexName(targetIndexName))
                .PrettyJson();

            var elasticClient = new ElasticClient(connectionSettings);

            var indexExistsResponse = elasticClient.Indices.Exists(targetIndexName);
            if (indexExistsResponse.IsValid && !indexExistsResponse.Exists)
            {
                // index does not exist, create it
                CreateIndex(elasticClient, targetIndexName);
            }

            var repository = new ElasticsearchRepository(elasticClient);

            return repository;
        }

        protected static string GetIndexName()
        {
            var documentType = typeof(T);
            var mapToTableAttributes = documentType.GetCustomAttributes(typeof(MapToIndexAttribute)).ToList();

            if (!mapToTableAttributes.Any()) return null;
            var firstAttribute = mapToTableAttributes.FirstOrDefault();

            if (!(firstAttribute is MapToIndexAttribute mapToTableAttribute)) return null;
            var name = mapToTableAttribute.Name;
            return name;
        }

        private static string GetHandledTableName()
        {
            var documentType = typeof(T);
            var mapToTableAttributes = documentType.GetCustomAttributes(typeof(MapToTableAttribute)).ToList();

            if (!mapToTableAttributes.Any()) return null;
            var firstAttribute = mapToTableAttributes.FirstOrDefault();

            if (!(firstAttribute is MapToTableAttribute mapToTableAttribute)) return null;
            var name = mapToTableAttribute.Name;

            return name;
        }
    }
}