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
            return query.Where(x => x.PartitionKey == partitionKey);
        }
    }
}
