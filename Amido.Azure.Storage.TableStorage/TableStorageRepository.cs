using System;
using System.Collections.Generic;
using System.Linq;
using Amido.Azure.Storage.TableStorage.Account;
using Amido.Azure.Storage.TableStorage.Dbc;
using Amido.Azure.Storage.TableStorage.Paging;
using Amido.Azure.Storage.TableStorage.Serialization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    /// <summary>
    /// Class TableStorageRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public class TableStorageRepository<TEntity> : ITableStorageRepository<TEntity>, ITableStorageAdminRepository where TEntity : class, ITableEntity, new()
    {
        private readonly string tableName;
        private readonly CloudTableClient cloudTableClient;
        private readonly BatchWriter<TEntity> batchWriter;
        private readonly CompensatingBatchWriter<TEntity> compensatingBatchWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="accountConfiguration">The account configuration.</param>
        public TableStorageRepository(AccountConfiguration<TEntity> accountConfiguration)
            : this(GetCloudStorageAccountByConfigurationSetting(accountConfiguration.AccountName), accountConfiguration.TableName) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="accountConnection">The account connection.</param>
        public TableStorageRepository(AccountConnection<TEntity> accountConnection)
            : this(GetCloudStorageAccountByConnectionString(accountConnection.ConnectionString), accountConnection.TableName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="cloudStorageAccount">The cloud storage account.</param>
        /// <param name="tableName">Name of the table.</param>
        public TableStorageRepository(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            this.tableName = tableName;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            batchWriter = new BatchWriter<TEntity>(cloudStorageAccount, tableName);
            compensatingBatchWriter = new CompensatingBatchWriter<TEntity>(this, cloudStorageAccount, tableName);
        }

        /// <summary>
        /// Returns an instance of the <see cref="BatchWriter"/> class.  This should be used
        /// when performing batch operations.
        /// </summary>
        public IBatchWriter<TEntity> BatchWriter 
        {
            get { return batchWriter; }
        }

        /// <summary>
        /// Returns an instance of the <see cref="BatchWriter"/> class.  This should be used
        /// when performing batch operations.
        /// </summary>
        public ICompensatingBatchWriter<TEntity> CompensatingBatchWriter 
        {
            get { return compensatingBatchWriter; }
        }

        /// <summary>
        /// Returns a reference to table.
        /// </summary>
        public CloudTable Table
        {
            get
            {
                return cloudTableClient == null ? null : cloudTableClient.GetTableReference(tableName);
            }
        }

        /// <summary>
        /// Executes the table query (with optional continuation token) and returns a maximum of 1000 entities in a paged results.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        public PagedResults<TEntity> Query(TableQuery<TEntity> query, string continuationToken = null)
        {
            Contract.Requires(query != null, "query is null");

            var serializer = new ContinuationTokenSerializer();

            var token = string.IsNullOrWhiteSpace(continuationToken)
                                               ? null
                                               : serializer.DeserializeToken(continuationToken);

            var tableQuerySegment = Table.ExecuteQuerySegmented(query, token);

            return CreatePagedResults(tableQuerySegment, serializer);
        }

        /// <summary>
        /// Executes the table query (with optional continuation token) and returns the supplied number of entities in a paged results.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        public PagedResults<TEntity> Query(TableQuery<TEntity> query, int resultsPerPage, string continuationToken = null) 
        {
            Contract.Requires(query != null, "query is null");
            Contract.Requires(resultsPerPage > 0, "resultsPerPage is zero or less");

            var serializer = new ContinuationTokenSerializer();

            var token = string.IsNullOrWhiteSpace(continuationToken)
                                               ? null
                                               : serializer.DeserializeToken(continuationToken);

            var tableQuerySegment = Table.ExecuteQuerySegmented(query.Take(resultsPerPage), token);

            return CreatePagedResults(tableQuerySegment, serializer);
        }

        /// <summary>
        /// Returns the first item matching the query, or null of none found.
        /// </summary>
        /// <remarks>Handles continuation tokens (see http://blog.smarx.com/posts/windows-azure-tables-expect-continuation-tokens-seriously )</remarks>
        /// <param name="query">The query.</param>
        /// <returns>The first item found or null if none.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        public TEntity FirstOrDefault(TableQuery<TEntity> query)
        {
            Contract.Requires(query != null, "query is null.");

            var results = Query(query.Take(1));
            while (!results.Results.Any() && results.HasMoreResults)
            {
                results = Query(query.Take(1));
            }

            return results.Results.FirstOrDefault();
        }

        /// <summary>
        /// Returns the first item matching the query.
        /// </summary>
        /// <remarks>Handles continuation tokens (see http://blog.smarx.com/posts/windows-azure-tables-expect-continuation-tokens-seriously )</remarks>
        /// <param name="query">The query.</param>
        /// <returns>The first item found.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        /// <exception cref="InvalidOperationException">If not result are found matching the query.</exception>
        public TEntity First(TableQuery<TEntity> query)
        {
            Contract.Requires(query != null, "query is null.");

            var results = Query(query.Take(1));
            while (!results.Results.Any() && results.HasMoreResults)
            {
                results = Query(query.Take(1));
            }

            return results.Results.First();
        }

        /// <summary>
        /// Returns an entity based upon partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="rowKey">The row key.</param>
        /// <returns>An instance of <typeparamref name="TEntity"/></returns>
        /// <exception cref="PreconditionException">partitionKey or rowKey are null or empty.</exception>
        public TEntity GetByPartitionKeyAndRowKey(string partitionKey, string rowKey)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(partitionKey), "partitionKey is null.");
            Contract.Requires(!string.IsNullOrWhiteSpace(rowKey), "rowKey is null.");
            
            var tableOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);

            var specificEntity = (TEntity)Table.Execute(tableOperation).Result;

            return specificEntity;
        }

        /// <summary>
        /// Returns a paged list of results based upon a partition key. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="resultsPerPage">The results per page.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        /// <exception cref="PreconditionException">If partitionKey is null or empty or resultsPerPage is less than one.</exception>
        public PagedResults<TEntity> ListByPartitionKey(string partitionKey, string continuationToken = null, int resultsPerPage = 1000)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(partitionKey), "partitionKey is null.");

            var query = new TableQuery<TEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey)).Take(resultsPerPage);

            return Query(query, resultsPerPage, continuationToken);
        }

        /// <summary>
        /// Returns a paged list of results. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        /// <exception cref="PreconditionException">If resultsPerPage is less than one.</exception>
        public PagedResults<TEntity> ListAll(string continuationToken = null)
        {
            return Query(new TableQuery<TEntity>(), continuationToken);
        }

        /// <summary>
        /// Returns a paged list of results. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="resultsPerPage">The result per page.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        /// <exception cref="PreconditionException">If resultsPerPage is less than one.</exception>
        public PagedResults<TEntity> ListAll(int resultsPerPage, string continuationToken = null) 
        {
            Contract.Requires(resultsPerPage > 0, "resultsPerPage is zero or less");

            return Query(new TableQuery<TEntity>(), resultsPerPage, continuationToken);
        }

        /// <summary>
        /// Gets all entities withing single partitions, handling continuation tokens to retrieve the full list. USE WITH CARE.
        /// </summary>
        /// <param name="partitionKey">The partition key</param>
        /// <returns>Enumerated entities</returns>
        public IEnumerable<TEntity> GetAllByPartitionKey(string partitionKey)
        {
            var results = ListByPartitionKey(partitionKey);
            var allEntities = results.Results;
            while (results.HasMoreResults)
            {
                results = ListByPartitionKey(partitionKey, results.ContinuationToken);
                allEntities.AddRange(results.Results);
            }
            return allEntities;
        }

        /// <summary>
        /// Gets all entities accross all partitions, handling continuation tokens to retrieve the full list. USE WITH CARE.
        /// </summary>
        /// <returns>Enumerated entities</returns>
        public IEnumerable<TEntity> GetAll()
        {
            var results = ListAll();
            var allEntities = results.Results;
            while (results.HasMoreResults)
            {
                results = ListAll(results.ContinuationToken);
                allEntities.AddRange(results.Results);
            }
            return allEntities;
        }

        /// <summary>
        /// Adds the specified entity into table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        public void Add(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            var operation = TableOperation.Insert(entity);
            Table.Execute(operation, GetTableRequestOptions());
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        public void Update(TEntity entity) 
        {
            Contract.Requires(entity != null, "entity is null");

            var operation = TableOperation.Replace(entity);
            Table.Execute(operation, GetTableRequestOptions());
        }

        /// <summary>
        /// Inserts the given entity into a table if the entity does not exist.
        /// If the entity does exist then its contents are replaced with the provided entity
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        public void InsertOrReplace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            var operation = TableOperation.InsertOrReplace(entity);
            Table.Execute(operation, GetTableRequestOptions());
        }

        /// <summary>
        /// Inserts the given entity into a table if the entity does not exist. 
        /// If the entity does exist then its contents are merged with the provided entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        public void InsertOrMerge(TEntity entity) 
        {
            Contract.Requires(entity != null, "entity is null");

            var operation = TableOperation.InsertOrMerge(entity);
            Table.Execute(operation, GetTableRequestOptions());
        }

        /// <summary>
        /// Deletes the specified entity from the table. 
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            var operation = TableOperation.Delete(entity);
            Table.Execute(operation, GetTableRequestOptions());
        }

        /// <summary>
        /// Creates the table if not exists.
        /// </summary>
        public void CreateTableIfNotExists()
        {
            Table.CreateIfNotExists();
        }

        /// <summary>
        /// Deletes a table from Table storage.
        /// </summary>
        public void DeleteTable()
        {
            Table.DeleteIfExists();
        }

        /// <summary>
        /// Returns a <see cref="TableRequestOptions"/> class to allow for setting the Retry policy for table operations.
        /// </summary>
        /// <returns></returns>
        private static TableRequestOptions GetTableRequestOptions() 
        {
            return new TableRequestOptions
            {
                RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(2), 100)
            };
        }

        private PagedResults<TEntity> CreatePagedResults(TableQuerySegment<TEntity> querySegment, ContinuationTokenSerializer serializer) 
        {
            var pagedResults = new PagedResults<TEntity>();
            pagedResults.Results.AddRange(querySegment.Results);
            pagedResults.ContinuationToken = querySegment.ContinuationToken == null
                                                 ? null
                                                 : serializer.SerializeToken(querySegment.ContinuationToken);
            pagedResults.HasMoreResults = querySegment.ContinuationToken != null;
            return pagedResults;
        }

        /// <summary>
        /// Gets the cloud storage account by connection string.
        /// </summary>
        /// <param name="storageConnectionString">The storage connection string.</param>
        /// <returns>CloudStorageAccount.</returns>
        /// <exception cref="System.InvalidOperationException">Unable to find cloud storage account</exception>
        private static CloudStorageAccount GetCloudStorageAccountByConnectionString(string storageConnectionString)
        {
            try
            {
                return CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (Exception error)
            {
                throw new InvalidOperationException("Unable to find cloud storage account", error);
            }
        }

        /// <summary>
        /// Gets the cloud storage account by configuration setting.
        /// </summary>
        /// <param name="configurationSetting">The configuration setting.</param>
        /// <returns>CloudStorageAccount.</returns>
        /// <exception cref="System.InvalidOperationException">Unable to find cloud storage account</exception>
        private static CloudStorageAccount GetCloudStorageAccountByConfigurationSetting(string configurationSetting) 
        {
            try 
            {
                return CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(configurationSetting));
            }
            catch(Exception error) 
            {
                throw new InvalidOperationException("Unable to find cloud storage account", error);
            }
        }
    }
}