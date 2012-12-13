using System.Collections.Generic;

namespace Amido.Azure.Storage.TableStorage.Paging
{
    /// <summary>
    /// Class PagedResults
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public class PagedResults<TEntity>
    {
        /// <summary>
        /// Gets or sets the list of results.
        /// </summary>
        /// <value>The results.</value>
        public List<TEntity> Results { get; set; }

        /// <summary>
        /// Gets or sets the continuation token.
        /// </summary>
        /// <value>The continuation token.</value>
        /// <remarks>The continuation token will be null if there are no more results available. If not null, then more results are available, and the token needs to be passed in subsequent calls for further pages</remarks>
        public string ContinuationToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has more results.
        /// </summary>
        /// <value><c>true</c> if this instance has more results; otherwise, <c>false</c>.</value>
        /// <remarks>If more results are available, this value will be true, and a ContinuationToken should be present.</remarks>
        public bool HasMoreResults { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedResults{TEntity}" /> class.
        /// </summary>
        public PagedResults()
        {
            Results = new List<TEntity>();
        }
    }
}
