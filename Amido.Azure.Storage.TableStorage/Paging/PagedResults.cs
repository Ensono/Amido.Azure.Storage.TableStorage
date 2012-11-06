using System.Collections.Generic;

namespace Amido.Azure.Storage.TableStorage.Paging
{
    public class PagedResults<TEntity>
    {
        public List<TEntity> Results { get; set; }
        public string ContinuationToken { get; set; }

        public PagedResults()
        {
            Results = new List<TEntity>();
        }
    }
}
