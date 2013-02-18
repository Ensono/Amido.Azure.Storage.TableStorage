using System.Collections.Generic;
using Amido.Azure.Storage.TableStorage.Paging;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage
{
    public static class Extensions
    {
        public static IList<TEntity> All<TEntity>(this PagedResults<TEntity> results) where TEntity : TableServiceEntity
        {
            if(results == null)
            {
                return null;
            }

            string continuationToken = null;
            var entities = new List<TEntity>();
            do 
            {
                var pagedResults = results;
                continuationToken = pagedResults.ContinuationToken;
                entities.AddRange(pagedResults.Results);

            } while(continuationToken != null);

            return entities;
        }
    }
}