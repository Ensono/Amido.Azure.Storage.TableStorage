using System;
using System.Linq;
using System.Linq.Expressions;
using Amido.Azure.Storage.TableStorage.Dbc;
using Amido.Azure.Storage.TableStorage.Paging;
using Amido.Azure.Storage.TableStorage.Queries;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage
{
    /// <summary>
    /// Interface ITableStorageRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public interface ITableStorageRepository<TEntity> where TEntity : TableServiceEntity
    {
        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Attaches the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void AttachEntity(TEntity entity);

        /// <summary>
        /// Attaches the entity for upsert.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void AttachEntityForUpsert(TEntity entity);

        /// <summary>
        /// Detaches the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void DetachEntity(TEntity entity);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Saves the batch.
        /// </summary>
        void SaveBatch();

        /// <summary>
        /// Saves the and replace on update.
        /// </summary>
        void SaveAndReplaceOnUpdate();

        /// <summary>
        /// Queries against a table and returns a <see cref="PagedResults{TEntity}"/> of results.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>PagedResults{`0}.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        PagedResults<TEntity> Query(Query<TEntity> query);

        /// <summary>
        /// Queries against a table and returns a <see cref="PagedResults{TEntity}" /> of results.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="resultsPerPage">The results per page.</param>
        /// <returns>PagedResults{`0}.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        PagedResults<TEntity> Query(Query<TEntity> query, int resultsPerPage);

        /// <summary>
        /// Returns the first item matching the query, or null of none found.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The first item found or null if none.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        TEntity FirstOrDefault(Query<TEntity> query);

        /// <summary>
        /// Returns the first item matching the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The first item found.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        /// <exception cref="InvalidOperationException">If not result are found matching the query.</exception>
        TEntity First(Query<TEntity> query);

        /// <summary>
        /// Returns an entity based upon partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <returns>An instance of <typeparamref name="TEntity"/></returns>
        /// <exception cref="PreconditionException">partitionKey or rowKey are null or empty.</exception>
        TEntity GetByPartitionKeyAndRowKey(string partitionKey, string rowKey);

        /// <summary>
        /// Returns a paged list of results based upon a partition key. If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        PagedResults<TEntity> ListByPartitionKey(string partitionKey, string continuationToken = null);

        /// <summary>
        /// Returns a paged list of results based upon a partition key. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="resultPerPage">The result per page.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        PagedResults<TEntity> ListByPartitionKey(string partitionKey, int resultPerPage, string continuationToken = null);

        /// <summary>
        /// Returns a paged list of results. If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        PagedResults<TEntity> ListAll(string continuationToken = null);

        /// <summary>
        /// Returns a paged list of results. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="resultPerPage">The result per page.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        PagedResults<TEntity> ListAll(int resultPerPage, string continuationToken = null);

        /// <summary>
        /// Finds results based upon a given expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

        /// <summary>
        /// Finds results based upon a given <see cref="Query{TEntity}"/> instance.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        IQueryable<TEntity> Find(Query<TEntity> query);

        /// <summary>
        /// Finds results based upon a given <see cref="Query{TEntity}"/> instance. Results can be limited by specifying the resultsPerPage to return.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="resultsPerPage">The results per page.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        IQueryable<TEntity> Find(Query<TEntity> query, int resultsPerPage);
    }
}
