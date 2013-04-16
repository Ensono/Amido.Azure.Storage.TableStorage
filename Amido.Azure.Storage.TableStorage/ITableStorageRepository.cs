using System;
using Amido.Azure.Storage.TableStorage.Dbc;
using Amido.Azure.Storage.TableStorage.Paging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    /// <summary>
    /// Interface ITableStorageRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public interface ITableStorageRepository<TEntity> where TEntity : class, ITableEntity, new()
    {
        /// <summary>
        /// Returns an instance of the <see cref="BatchWriter"/> class.  This should be used
        /// when performing batch operations.
        /// </summary>
        IBatchWriter<TEntity> BatchWriter { get; }

        /// <summary>
        /// Returns an instance of the <see cref="CompensatingBatchWriter"/> class.  This should be used
        /// when performing batch operations.
        /// </summary>
        ICompensatingBatchWriter<TEntity> CompensatingBatchWriter { get; }

        /// <summary>
        /// Returns a reference to table.
        /// </summary>
        CloudTable Table { get; }

        /// <summary>
        /// Adds the specified entity into table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        void Add(TEntity entity);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        void Update(TEntity entity);

        /// <summary>
        /// Inserts the given entity into a table if the entity does not exist.
        /// If the entity does exist then its contents are replaced with the provided entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        void InsertOrReplace(TEntity entity);

        /// <summary>
        /// Inserts the given entity into a table if the entity does not exist. 
        /// If the entity does exist then its contents are merged with the provided entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        void InsertOrMerge(TEntity entity);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(TEntity entity);
 
        /// <summary>
        /// Queries against a table and returns a <see cref="PagedResults{TEntity}" /> of results.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="resultsPerPage">The results per page.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{`0}.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        PagedResults<TEntity> Query(TableQuery<TEntity> query, int resultsPerPage, string continuationToken = null);

        /// <summary>
        /// Queries against a table and returns a <see cref="PagedResults{TEntity}" /> of results.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{`0}.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        PagedResults<TEntity> Query(TableQuery<TEntity> query, string continuationToken = null);

        /// <summary>
        /// Returns the first item matching the query, or null of none found.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The first item found or null if none.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        TEntity FirstOrDefault(TableQuery<TEntity> query);

        /// <summary>
        /// Returns the first item matching the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The first item found.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        /// <exception cref="InvalidOperationException">If not result are found matching the query.</exception>
        TEntity First(TableQuery<TEntity> query);

        /// <summary>
        /// Returns an entity based upon partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <returns>An instance of <typeparamref name="TEntity"/></returns>
        /// <exception cref="PreconditionException">partitionKey or rowKey are null or empty.</exception>
        TEntity GetByPartitionKeyAndRowKey(string partitionKey, string rowKey);

        /// <summary>
        /// Returns a paged list of results based upon a partition key. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="resultPerPage">The result per page.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        PagedResults<TEntity> ListByPartitionKey(string partitionKey, string continuationToken = null, int resultPerPage = 1000);

        /// <summary>
        /// Returns a paged list of results. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        PagedResults<TEntity> ListAll(string continuationToken = null);

        /// <summary>
        /// Returns a paged list of results. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="resultsPerPage">The result per page.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        PagedResults<TEntity> ListAll(int resultsPerPage, string continuationToken = null); 
    }
}
