using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Nest;
using Syncer.Elasticsearch.Abstractions;
using Syncer.Elasticsearch.Abstractions.Queries;
using Syncer.Elasticsearch.Extensions;
using Syncer.Elasticsearch.Queries;
using Syncer.Elasticsearch.Queries.Common;

namespace Syncer.Elasticsearch
{
    public partial class ElasticsearchRepository
    {
        public async Task<T> FindByIdAsync<T>(string id, string index = null) where T : class
        {
            var documentPath = DocumentPath<T>.Id(id);
            return await FindByIdAsync(documentPath, GetIndexName(_client, index));
        }

        public async Task<T> FindByIdAsync<T>(DocumentPath<T> documentPath, string index = null) where T : class
        {
            var response = await GetByIdAsync(documentPath, GetIndexName(_client, index));
            if (response.IsValid && response.Source != default)
            {
                return response.Source;
            }
            return null;
        }

        public async Task<IGetResponse<T>> GetByIdAsync<T>(string id, string index = null) where T : class
        {
            var documentPath = DocumentPath<T>.Id(id);
            return await QueryAsync(new GetByIdQuery<T>(documentPath), GetIndexName(_client, index));
        }

        public async Task<IGetResponse<T>> GetByIdAsync<T>(DocumentPath<T> documentPath, string index = null) where T : class
        {
            return await QueryAsync(new GetByIdQuery<T>(documentPath), GetIndexName(_client, index));
        }

        public async Task<TResponse> QueryAsync<TResponse>(IElasticClientQueryObject<TResponse> query, string index = null) where TResponse : class
        {
            return await _client.QueryAsync(query, GetIndexName(_client, index));
        }

        public async Task<IndexResponse> SaveAsync<T>(T document, string index = null, bool? refreshOnSave = null) where T : class
        {
            if (document == null) throw new ArgumentNullException(nameof(document), "indexed document can not be null");

            return await QueryAsync(new IndexDocumentQuery<T>(document, refreshOnSave.GetValueOrDefault(false)), GetIndexName(_client, index));
        }

        public async Task<BulkResponse> BulkAsync<T>(IEnumerable<T> documents, string index = null, bool? refreshOnSave = null) where T : class
        {
            return await QueryAsync(new BulkIndexDocumentQuery<T>(documents, refreshOnSave.GetValueOrDefault(false)), GetIndexName(_client, index));

        }

        public async Task<DeleteResponse> DeleteAsync<T>(T document, string index = null, bool? refreshOnDelete = null) where T : class
        {
            return await QueryAsync(new DeleteDocumentQuery<T>(document, refreshOnDelete.GetValueOrDefault(false)), GetIndexName(_client, index));
        }

        public async Task<DeleteResponse> DeleteAsync<T>(string id, string index = null, bool? refreshOnDelete = null) where T : class
        {
            return await QueryAsync(new DeleteByIdQuery<T>(id, refreshOnDelete.GetValueOrDefault(false)), GetIndexName(_client, index));
        }

        public async Task<bool> ExistsAsync<T>(T document, string index = null) where T : class
        {
            var response = await QueryAsync(new DocumentExistsQuery<T>(document), GetIndexName(_client, index));
            return response.IsValid && response.Exists;
        }

        public async Task<bool> ExistsAsync<T>(string id, string index = null) where T : class
        {
            var response = await QueryAsync(new DocumentExistsByIdQuery<T>(id), GetIndexName(_client, index));
            return response.IsValid && response.Exists;
        }
    }

    [DebuggerStepThrough]
    public partial class ElasticsearchRepository : IElasticsearchRepository
    {
        private readonly IElasticClient _client;

        public ElasticsearchRepository(IElasticClient client)
        {
            _client = client;
        }

        protected virtual string GetIndexName(IElasticClient client, string index = null)
        {
            return index ?? client.GetDefaultIndex();
        }

        public T FindById<T>(string id, string index = null) where T : class
        {
            var documentPath = DocumentPath<T>.Id(id);
            return FindById(documentPath, GetIndexName(_client, index));
        }

        public T FindById<T>(DocumentPath<T> documentPath, string index = null) where T : class
        {
            var response = GetById(documentPath, GetIndexName(_client, index));
            if (response.IsValid && response.Source != default)
            {
                return response.Source;
            }
            return null;
        }

        public IGetResponse<T> GetById<T>(string id, string index = null) where T : class
        {
            var documentPath = DocumentPath<T>.Id(id);
            return GetById(documentPath, index);
        }

        public IGetResponse<T> GetById<T>(DocumentPath<T> documentPath, string index = null) where T : class
        {
            return Query(new GetByIdQuery<T>(documentPath), GetIndexName(_client, index));
        }

        public TResponse Query<TResponse>(IElasticClientQueryObject<TResponse> query, string index = null)
            where TResponse : class
        {
            return _client.Query(query, GetIndexName(_client, index));
        }

        /// <exception cref="NullReferenceException">indexed document can not be null</exception>
        public IndexResponse Save<T>(T document, string index = null, bool? refreshOnSave = null) where T : class
        {
            if (document == null) throw new ArgumentNullException(nameof(document), "indexed document can not be null");

            return Query(new IndexDocumentQuery<T>(document, refreshOnSave.GetValueOrDefault(false)), GetIndexName(_client, index));
        }

        public BulkResponse Bulk<T>(IEnumerable<T> documents, string index = null, bool? refreshOnSave = null) where T : class
        {
            return Query(new BulkIndexDocumentQuery<T>(documents, refreshOnSave.GetValueOrDefault(false)), GetIndexName(_client, index));
        }

        public DeleteResponse Delete<T>(T document, string index = null, bool? refreshOnDelete = null) where T : class
        {
            return Query(new DeleteDocumentQuery<T>(document, refreshOnDelete.GetValueOrDefault(false)), GetIndexName(_client, index));
        }

        public DeleteResponse Delete<T>(string id, string index = null, bool? refreshOnDelete = null) where T : class
        {
            return Query(new DeleteByIdQuery<T>(id, refreshOnDelete.GetValueOrDefault(false)), GetIndexName(_client, index));
        }

        public bool Exists<T>(string id, string index = null) where T : class
        {
            var response = Query(new DocumentExistsByIdQuery<T>(id), GetIndexName(_client, index));
            return response.IsValid && response.Exists;
        }

        public bool Exists<T>(T document, string index = null) where T : class
        {
            var response = Query(new DocumentExistsQuery<T>(document), GetIndexName(_client, index));
            return response.IsValid && response.Exists;
        }
    }
}