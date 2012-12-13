using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Queries
{
    internal class ListAllQuery<TEntity> : Query<TEntity> where TEntity : TableServiceEntity
    {
        public override IQueryable<TEntity> Execute(IQueryable<TEntity> query) 
        {
            return Execute(query, 0);
        }

        public override IQueryable<TEntity> Execute(IQueryable<TEntity> query, int resultsPerPage) 
        {
            return resultsPerPage > 0 ? query.Take(resultsPerPage) : query;
        }
    }
}