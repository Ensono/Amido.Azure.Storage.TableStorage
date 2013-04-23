using System.Linq;

namespace Amido.Azure.Storage.TableStorage
{
    public interface IQueryableRepository<TEntity>
    {
        IQueryable<TEntity> GetByPartition(string partitionKey);
    }
}