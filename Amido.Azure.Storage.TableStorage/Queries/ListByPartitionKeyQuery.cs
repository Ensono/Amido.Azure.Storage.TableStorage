using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Queries
{
    internal class ListByPartitionKeyQuery<TEntity> : Query<TEntity> where TEntity : TableServiceEntity
    {
        private readonly string partitionKey;

        public ListByPartitionKeyQuery(string partitionKey)
        {
            this.partitionKey = partitionKey;
        }

        public override IQueryable<TEntity> Execute(IQueryable<TEntity> query)
        {
            return Execute(query, 0);
        }

        public override IQueryable<TEntity> Execute(IQueryable<TEntity> query, int resultsPerPage)
        {
            query = query.Where(x => x.PartitionKey == partitionKey);

            return resultsPerPage > 0 ? query.Take(resultsPerPage) : query;
        }
    }
}