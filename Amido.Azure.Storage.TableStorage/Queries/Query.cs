using System.Linq;

namespace Amido.Azure.Storage.TableStorage.Queries
{
    public abstract class Query<TEntity>
    {
        public string ContinuationTokenString { get; set; }

        public abstract IQueryable<TEntity> Execute(IQueryable<TEntity> query);
    }
}
