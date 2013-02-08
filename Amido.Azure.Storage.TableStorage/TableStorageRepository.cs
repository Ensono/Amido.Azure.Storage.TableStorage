using System;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;
using Amido.Azure.Storage.TableStorage.Account;
using Amido.Azure.Storage.TableStorage.Dbc;
using Amido.Azure.Storage.TableStorage.Paging;
using Amido.Azure.Storage.TableStorage.Serialization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    /// <summary>
    /// Class TableStorageRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the T entity.</typeparam>
    public class TableStorageRepository<TEntity> : TableEntity, ITableStorageRepository<TEntity>, ITableStorageAdminRepository where TEntity : ITableEntity, new()
    {
        private readonly string tableName;
        private readonly CloudTableClient cloudTableClient;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="TableStorageRepository{TEntity}" /> class.
        ///// </summary>
        ///// <param name="accountConfiguration">The account configuration.</param>
        //public TableStorageRepository(AccountConfiguration<TEntity> accountConfiguration)
        //    : this(GetCloudStorageAccountByConfigurationSetting(accountConfiguration.AccountName), accountConfiguration.TableName) {
        //}

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
            : base(cloudStorageAccount.TableEndpoint.AbsoluteUri, cloudStorageAccount.Credentials.AccountName)
        {
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            this.tableName = tableName;
            //MergeOption = MergeOption.PreserveChanges;
            //IgnoreResourceNotFoundException = true;
        }

        public PagedResults<TEntity> Query(TableQuery<TEntity> query, string continuationToken = null)
        {

            Contract.Requires(query != null, "query is null");
            var table = cloudTableClient.GetTableReference(tableName);

            var serializer = new ContinuationTokenSerializer();

            var token = string.IsNullOrWhiteSpace(continuationToken)
                                               ? null
                                               : serializer.DeserializeToken(continuationToken);

            var tableQuerySegment = table.ExecuteQuerySegmented(query, token);

            return CreatePagedResults(tableQuerySegment, serializer);
        }

        public PagedResults<TEntity> Query(TableQuery<TEntity> query, int resultsPerPage, string continuationToken = null)
        {
            Contract.Requires(query != null, "query is null");
            var table = cloudTableClient.GetTableReference(tableName);

            var serializer = new ContinuationTokenSerializer();

            var token = string.IsNullOrWhiteSpace(continuationToken)
                                               ? null
                                               : serializer.DeserializeToken(continuationToken);

            var tableQuerySegment = table.ExecuteQuerySegmented(query.Take(resultsPerPage), token);

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
            var table = cloudTableClient.GetTableReference(tableName);
            return table.ExecuteQuery(query).FirstOrDefault();
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
            var table = cloudTableClient.GetTableReference(tableName);
            return table.ExecuteQuery(query).FirstOrDefault();
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
            
            var table = cloudTableClient.GetTableReference(tableName);

            var tableOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);

            var specificEntity = (TEntity)table.Execute(tableOperation).Result;

            return specificEntity;
        }

        /// <summary>
        /// Returns a paged list of results based upon a partition key. If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        /// <exception cref="PreconditionException">If partitionKey is null or empty.</exception>
        public PagedResults<TEntity> ListByPartitionKey(string partitionKey, string continuationToken = null)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(partitionKey), "partitionKey is null.");

            var query = new TableQuery<TEntity>().Where(TableQuery.GenerateFilterCondition("partitionKey", QueryComparisons.Equal, partitionKey));

            return Query(query, continuationToken);
        }

        /// <summary>
        /// Returns a paged list of results based upon a partition key. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="partitionKey">The partition key.</param>
        /// <param name="resultsPerPage">The results per page.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        /// <exception cref="PreconditionException">If partitionKey is null or empty or resultsPerPage is less than one.</exception>
        public PagedResults<TEntity> ListByPartitionKey(string partitionKey, int resultsPerPage, string continuationToken = null)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(partitionKey), "partitionKey is null.");

            var query = new TableQuery<TEntity>().Where(TableQuery.GenerateFilterCondition("partitionKey", QueryComparisons.Equal, partitionKey)).Take(resultsPerPage);

            return Query(query, continuationToken);
        }

        /// <summary>
        /// Returns a paged list of results. If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="continuationToken">The continuation token.</param>
        /// <returns>PagedResults{TEntity}.</returns>
        public PagedResults<TEntity> ListAll(string continuationToken = null)
        {
            return Query(new TableQuery<TEntity>(), continuationToken);
        }

        /// <summary>
        /// Returns a paged list of results. The number of results returned can be constrained by passing a value for resultsPerPage.
        /// If a continuationToken is passed, it will return the next page of results.
        /// </summary>
        /// <param name="resultsPerPage">The result per page.</param>
        /// <param name="continuationToken">The continuation token.</param>
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
            var table = cloudTableClient.GetTableReference(tableName);

            var query = new TableQuery<TEntity>();

            return table.ExecuteQuery(query).Where(expression.Compile()).AsQueryable();
        }

        /// <summary>
        /// Finds results based upon a given <see cref="Query{TEntity}"/> instance.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        /// <exception cref="PreconditionException">If query is null.</exception>
        public IQueryable<TEntity> Find(TableQuery<TEntity> query)
        {
            Contract.Requires(query != null, "query is null.");
            var table = cloudTableClient.GetTableReference(tableName);
            return table.ExecuteQuery(query).AsQueryable();
        }

        /// <summary>
        /// Finds results based upon a given <see cref="Query{TEntity}"/> instance. Results can be limited by specifying the resultsPerPage to return.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="resultsPerPage">The results per page.</param>
        /// <returns>IQueryable{TEntity}.</returns>
        /// <exception cref="PreconditionException">If query is null or resultsPerPage is less than one.</exception>
        public IQueryable<TEntity> Find(TableQuery<TEntity> query, int resultsPerPage) 
        {
            Contract.Requires(query != null, "query is null.");
            Contract.Requires(resultsPerPage > 0, "resultsPerPage is zero or less.");

            var table = cloudTableClient.GetTableReference(tableName);
            return table.ExecuteQuery(query.Take(resultsPerPage)).AsQueryable();
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        public virtual void Add(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");
            var tableServiceContext = cloudTableClient.GetTableServiceContext();
            tableServiceContext.AddObject(tableName, entity);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="PreconditionException">If entity is null.</exception>
        public virtual void Update(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");
            var tableServiceContext = cloudTableClient.GetTableServiceContext();
            tableServiceContext.UpdateObject(entity);
        }

        /// <summary>
        /// Attaches the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void AttachEntity(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");
            var tableServiceContext = cloudTableClient.GetTableServiceContext();
            tableServiceContext.AttachTo(tableName, entity);
        }

        /// <summary>
        /// Attaches the entity for upsert.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void AttachEntityForUpsert(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");
            var tableServiceContext = cloudTableClient.GetTableServiceContext();
            tableServiceContext.AttachTo(tableName, entity);
        }

        /// <summary>
        /// Detaches the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void DetachEntity(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");
            var tableServiceContext = cloudTableClient.GetTableServiceContext();
            tableServiceContext.Detach(entity);
        }

        /// <summary>
        /// Deletes the specified entity from the table.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");
            var tableServiceContext = cloudTableClient.GetTableServiceContext();
            tableServiceContext.DeleteObject(entity);
        }

        /// <summary>
        /// Saves any changes made to the table in question.
        /// </summary>
        public void SaveBatch()
        {
            //SaveChangesWithRetries(SaveChangesOptions.Batch);
        }

        /// <summary>
        /// Saves the and replace on update.
        /// </summary>
        public void SaveAndReplaceOnUpdate()
        {
            //var table = cloudTableClient.GetTableReference(tableName);
            //TableServiceContext ctx = cloudTableClient.GetTableServiceContext();
            
            //ctx.SaveChanges()
            //SaveChangesWithRetries(SaveChangesOptions.ReplaceOnUpdate);
        }

        /// <summary>
        /// Creates the table if not exists.
        /// </summary>
        public void CreateTableIfNotExists()
        {
            var table = cloudTableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
        }

        /// <summary>
        /// Deletes a table from Table storage.
        /// </summary>
        public void DeleteTable()
        {
            var table = cloudTableClient.GetTableReference(tableName);
            table.DeleteIfExists();
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

        ///// <summary>
        ///// Gets the cloud storage account by configuration setting.
        ///// </summary>
        ///// <param name="configurationSetting">The configuration setting.</param>
        ///// <returns>CloudStorageAccount.</returns>
        ///// <exception cref="System.InvalidOperationException">Unable to find cloud storage account</exception>
        //protected static CloudStorageAccount GetCloudStorageAccountByConfigurationSetting(string configurationSetting) {
        //    try {
        //        return CloudStorageAccount.FromConfigurationSetting(configurationSetting);
        //    }
        //    catch(Exception error) {
        //        throw new InvalidOperationException("Unable to find cloud storage account", error);
        //    }
        //}
    }
}