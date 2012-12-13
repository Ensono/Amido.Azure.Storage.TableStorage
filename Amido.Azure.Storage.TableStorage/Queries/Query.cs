using System.Linq;

namespace Amido.Azure.Storage.TableStorage.Queries
{
    /// <summary>
    /// Abstract base class for creating queries to execute against an Windows Azure storage table.
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public abstract class Query<TEntity> //where TEntity : TableServiceEntity
    {
        /// <summary>
        /// Gets or sets the continuation token string.
        /// </summary>
        /// <value>The continuation token string.</value>
        public string ContinuationTokenString { get; set; }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        public abstract IQueryable<TEntity> Execute(IQueryable<TEntity> query);

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="resultsPerPage">The results per page.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        public abstract IQueryable<TEntity> Execute(IQueryable<TEntity> query, int resultsPerPage);
    }
}
