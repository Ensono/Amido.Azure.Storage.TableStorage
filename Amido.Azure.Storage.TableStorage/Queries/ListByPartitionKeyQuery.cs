using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Queries
{
    internal class ListByPartitionKeyQuery<TEntity> : Query<TEntity> where TEntity : TableServiceEntity
    {
        private readonly string partitionKey;
        private readonly int resultsPerPage;

        public ListByPartitionKeyQuery(string partitionKey) : this(partitionKey, 0)
        {
        }

        public ListByPartitionKeyQuery(string partitionKey, int resultsPerPage)
        {
            this.partitionKey = partitionKey;
            this.resultsPerPage = resultsPerPage;
        }

        public override IQueryable<TEntity> Execute(IQueryable<TEntity> query)
        {
            query = query.Where(x => x.PartitionKey == partitionKey);

            return resultsPerPage > 0 ? query.Take(resultsPerPage) : query;
        }
    }
}
