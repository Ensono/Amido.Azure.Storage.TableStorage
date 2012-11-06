using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Queries
{
    internal class ListAllQuery<TEntity> : Query<TEntity> where TEntity : TableServiceEntity
    {
        public override IQueryable<TEntity> Execute(IQueryable<TEntity> query)
        {
            return query;
        }
    }
}
