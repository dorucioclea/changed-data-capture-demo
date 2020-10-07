﻿using System.Threading.Tasks;
using Nest;
using Syncer.Elasticsearch.Abstractions.Queries;

namespace Syncer.Elasticsearch.Extensions
{
    public static class ElasticClientQueryExtensions
    {
        public static TResponse Query<TResponse>(this IElasticClient client, IElasticClientQueryObject<TResponse> query, string index = null) where TResponse : class
        {
            return query.Execute(client, index);
        }

        public static async Task<TResponse> QueryAsync<TResponse>(this IElasticClient client, IElasticClientQueryObject<TResponse> query, string index = null) where TResponse : class
        {
            return await query.ExecuteAsync(client, index);
        }
    }
}
