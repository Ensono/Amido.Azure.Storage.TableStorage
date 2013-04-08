using System.Collections.Generic;
using Amido.Azure.Storage.TableStorage.Paging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public static class Extensions
    {
        public static IList<TEntity> All<TEntity>(this PagedResults<TEntity> results) where TEntity : class, ITableEntity, new()
        {
            if (results == null)
            {
                return null;
            }

            string continuationToken;
            var entities = new List<TEntity>();
            do
            {
                var pagedResults = results;
                continuationToken = pagedResults.ContinuationToken;
                entities.AddRange(pagedResults.Results);

            } while (continuationToken != null);

            return entities;
        }
    }
}