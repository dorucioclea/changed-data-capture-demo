using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;

namespace Syncer.Elasticsearch.Abstractions
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
    public interface IElasticsearchRepository : IElasticsearchRepositoryAsync
    {
        /// <summary>
        /// Get individual item for <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        T FindById<T>(string id, string index = null) where T : class;

        /// <summary>
        /// Get individual item for <paramref name="documentPath"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documentPath"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        T FindById<T>(DocumentPath<T> documentPath, string index = null) where T : class;

        /// <summary>
        /// Get individual item for <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        IGetResponse<T> GetById<T>(string id, string index = null) where T : class;

        IUpdateResponse<T> UpdateById<T>(string id, T document, string index = null, bool? refreshOnUpdate = null) where T : class;

        /// <summary>
        /// Get individual item for <paramref name="documentPath"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documentPath"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        IGetResponse<T> GetById<T>(DocumentPath<T> documentPath, string index = null) where T : class;

        /// <summary>
        /// Execute a query against the given or derived index
        /// </summary>
        /// <typeparam name="TResponse">The response associated with the query</typeparam>
        /// <param name="query">query object to execute</param>
        /// <param name="index">(optional) index on which to execute the query, if not supplied the index default index will be used</param>
        /// <returns>the response associated with the query</returns>
        TResponse Query<TResponse>(IElasticClientQueryObject<TResponse> query, string index = null)
            where TResponse : class;

        /// <summary>
        /// Saves an individual item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">the document to save</param>
        /// <param name="index">(optional) index on which to execute the query, if not supplied the index default index will be used</param>
        /// <param name="refreshOnSave">specifies whether to refresh the search index after completing the save operation, this will make the document immediately available to search, only use when you understand the impact</param>
        /// <returns></returns>
        IndexResponse Save<T>(T document, string index = null, bool? refreshOnSave = null) where T : class;
        /// <summary>
        /// Issues a bulk insert operation for all items in a single bulk batch
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="documents">the document batch to save</param>
        /// <param name="index">(optional) index on which to execute the query, if not supplied the index default index will be used</param>
        /// <param name="refreshOnSave">specifies whether to refresh the search index after completing the save operation, this will make the document immediately available to search, only use when you understand the impact</param>
        /// <returns></returns>
        /// <remarks>Manage your bulk batch size, do not flood the bulk api</remarks>
        BulkResponse Bulk<T>(IEnumerable<T> documents, string index = null, bool? refreshOnSave = null) where T : class;
        /// <summary>
        /// Removes an individual item by deriving the id from the given <paramref name="document"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <param name="index">(optional) index on which to execute the query, if not supplied the index default index will be used</param>
        /// <param name="refreshOnDelete">specifies whether to refresh the search index after completing the delete operation, only use when you understand the impact</param>
        /// <returns></returns>
        DeleteResponse Delete<T>(T document, string index = null, bool? refreshOnDelete = null) where T : class;
        /// <summary>
        /// Removes an individual item identified by <paramref name="id"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="index">(optional) index on which to execute the query, if not supplied the index default index will be used</param>
        /// <param name="refreshOnDelete">specifies whether to refresh the search index after completing the delete operation, only use when you understand the impact</param>
        /// <returns></returns>
        DeleteResponse Delete<T>(string id, string index = null, bool? refreshOnDelete = null) where T : class;
        /// <summary>
        /// Determines whether <paramref name="document"/> exists in the given index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">the document whose existence you wish to verify</param>
        /// <param name="index">(optional) index on which to execute the query, if not supplied the index default index will be used</param>
        /// <returns></returns>
        bool Exists<T>(T document, string index = null) where T : class;
        /// <summary>
        /// Determines whether an item identified by <paramref name="id"/> exists in the given index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">the identifier of the document whose existence you wish to verify</param>
        /// <param name="index">(optional) index on which to execute the query, if not supplied the index default index will be used</param>
        /// <returns></returns>
        bool Exists<T>(string id, string index = null) where T : class;
    }
}