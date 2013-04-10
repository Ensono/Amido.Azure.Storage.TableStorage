using System;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly Lazy<BatchWriter<TEntity>> batchWriter;
        private bool disposed;

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
        protected TableStorageRepository(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            this.tableName = tableName;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            batchWriter = new Lazy<BatchWriter<TEntity>>(() => new BatchWriter<TEntity>(cloudStorageAccount, tableName));
        }

        /// <summary>
        /// Returns an instance of the <see cref="BatchWriter"/> class.  This should be used
        /// when performing batch operations.
        /// </summary>
        public BatchWriter<TEntity> BatchWriter 
        {
            get { return batchWriter.Value; }
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
        /// 
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
        /// 
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
        /// <param name="query">The query.</param>
        /// <returns>The first item found or null if none.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        public TEntity FirstOrDefault(TableQuery<TEntity> query)
        {
            Contract.Requires(query != null, "query is null.");
            return Table.ExecuteQuery(query).FirstOrDefault();
        }

        /// <summary>
        /// Returns the first item matching the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The first item found.</returns>
        /// <exception cref="PreconditionException">If query object is null.</exception>
        /// <exception cref="InvalidOperationException">If not result are found matching the query.</exception>
        public TEntity First(TableQuery<TEntity> query)
        {
            Contract.Requires(query != null, "query is null.");
            return Table.ExecuteQuery(query).First();
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
            return Query(new TableQuery<TEntity>(), resultsPerPage, continuationToken);
        }

        /// <summary>
        /// Finds results based upon a given expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        /// <exception cref="PreconditionException">If expression is null.</exception>
        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        {
            Contract.Requires(expression!=null, "expression is null.");
            var query = new TableQuery<TEntity>();
            return Table.ExecuteQuery(query).Where(expression.Compile()).AsQueryable();
        }

        /// <summary>
        /// Finds results based upon a given <see cref="TableQuery{TEntity}"/> instance. Results can be limited by specifying the resultsPerPage to return.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        /// <exception cref="PreconditionException">If query is null or resultsPerPage is less than one.</exception>
        public IQueryable<TEntity> Find(TableQuery<TEntity> query) 
        {
            Contract.Requires(query != null, "query is null.");
            return Table.ExecuteQuery(query).AsQueryable();
        }

        /// <summary>
        /// Adds the specified entity into table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        public virtual void Add(TEntity entity)
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
        public virtual void Update(TEntity entity) 
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
        public virtual void InsertOrReplace(TEntity entity)
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
        public virtual void InsertOrMerge(TEntity entity) 
        {
            Contract.Requires(entity != null, "entity is null");
            var operation = TableOperation.InsertOrMerge(entity);
            Table.Execute(operation, GetTableRequestOptions());
        }

        ///// <summary>
        ///// Attaches the entity.
        ///// </summary>
        ///// <param name="entity">The entity.</param>
        //public virtual void AttachEntity(TEntity entity)
        //{
        //    Contract.Requires(entity != null, "entity is null");
        //    tableServiceContext.Value.AttachTo(tableName, entity);
        //}

        ///// <summary>
        ///// Attaches the entity for upsert.
        ///// </summary>
        ///// <param name="entity">The entity.</param>
        //public virtual void AttachEntityForUpsert(TEntity entity)
        //{
        //    Contract.Requires(entity != null, "entity is null");
        //    tableServiceContext.Value.AttachTo(tableName, entity);
        //}

        ///// <summary>
        ///// Detaches the entity.
        ///// </summary>
        ///// <param name="entity">The entity.</param>
        //public virtual void DetachEntity(TEntity entity)
        //{
        //    Contract.Requires(entity != null, "entity is null");
        //    tableServiceContext.Value.Detach(entity);
        //}

        /// <summary>
        /// Deletes the specified entity from the table. 
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(TEntity entity)
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
        protected static TableRequestOptions GetTableRequestOptions() 
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
        protected static CloudStorageAccount GetCloudStorageAccountByConnectionString(string storageConnectionString)
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
        protected static CloudStorageAccount GetCloudStorageAccountByConfigurationSetting(string configurationSetting) 
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