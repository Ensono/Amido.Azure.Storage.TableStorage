using System.Linq;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Queries
{
    internal class ListAllQuery<TEntity> : Query<TEntity> where TEntity : TableServiceEntity
    {
        private readonly int resultsPerPage;

        public ListAllQuery() : this(0)
        {
        }

        public ListAllQuery(int resultsPerPage)
        {
            this.resultsPerPage = resultsPerPage;
        }
        
        public override IQueryable<TEntity> Execute(IQueryable<TEntity> query)
        {
            return resultsPerPage > 0 ? query.Take(resultsPerPage) : query;
        }
    }
}
