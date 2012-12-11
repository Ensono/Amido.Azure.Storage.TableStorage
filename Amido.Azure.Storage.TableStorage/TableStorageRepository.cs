using System;
using System.Data.Services.Client;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization;
using Amido.Azure.Storage.TableStorage.Account;
using Amido.Azure.Storage.TableStorage.Paging;
using Amido.Azure.Storage.TableStorage.Queries;
using Amido.Azure.Storage.TableStorage.Serialization;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage
{
    public class TableStorageRepository<TEntity> : TableServiceContext, ITableStorageRepository<TEntity>, ITableStorageAdminRepository where TEntity : TableServiceEntity
    {
        private readonly string tableName;
        private readonly CloudTableClient cloudTableClient;

        public TableStorageRepository(AccountConfiguration<TEntity> accountConfiguration)
            : this(GetCloudStorageAccountByConfigurationSetting(accountConfiguration.AccountName), accountConfiguration.TableName)
        {
        }

        public TableStorageRepository(AccountConnection<TEntity> accountConnection)
            : this(GetCloudStorageAccountByConnectionString(accountConnection.ConnectionString), accountConnection.TableName)
        {
        }

        protected TableStorageRepository(CloudStorageAccount cloudStorageAccount, string tableName)
            : base(cloudStorageAccount.TableEndpoint.AbsoluteUri, cloudStorageAccount.Credentials)
        {
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            this.tableName = tableName;
            MergeOption = MergeOption.PreserveChanges;
            IgnoreResourceNotFoundException = true;
        }

        public PagedResults<TEntity> Query(Query<TEntity> query)
        {
            var serializer = new XmlSerializer(typeof(ResultContinuation));
            var cloudTableQuery = query.Execute(CreateQuery<TEntity>(tableName)).AsTableServiceQuery();
            var resultContinuation = ResultContinuationSerializer.DeserializeToken(serializer, query.ContinuationTokenString);
            var response = cloudTableQuery.EndExecuteSegmented(cloudTableQuery.BeginExecuteSegmented(resultContinuation, null, null));
            
            return CreatePagedResults(serializer, response);
        }

        public TEntity FirstOrDefault(Query<TEntity> query)
        {
            return query.Execute(CreateQuery<TEntity>(tableName)).FirstOrDefault();
        }

        public TEntity First(Query<TEntity> query)
        {
            return query.Execute(CreateQuery<TEntity>(tableName)).First();
        }

        public TEntity GetByPartitionKeyAndRowKey(string partitionKey, string rowKey)
        {
            return FirstOrDefault(new GetByPartitionKeyAndRowKeyQuery<TEntity>(partitionKey, rowKey));
        }

        public PagedResults<TEntity> ListByPartitionKey(string partitionKey, string continuationToken = null)
        {
            return Query(new ListByPartitionKeyQuery<TEntity>(partitionKey) { ContinuationTokenString = continuationToken });
        }

        public PagedResults<TEntity> ListByPartitionKey(string partitionKey, int resultsPerPage, string continuationToken = null)
        {
            return Query(new ListByPartitionKeyQuery<TEntity>(partitionKey, resultsPerPage) { ContinuationTokenString = continuationToken });
        }

        public PagedResults<TEntity> ListAll(string continuationToken = null)
        {
            return Query(new ListAllQuery<TEntity> { ContinuationTokenString = continuationToken });
        }

        public PagedResults<TEntity> ListAll(int resultsPerPage, string continuationToken = null)
        {
            return Query(new ListAllQuery<TEntity>(resultsPerPage) { ContinuationTokenString = continuationToken });
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        {
            return CreateQuery<TEntity>(tableName).Where(expression);
        }

        public IQueryable<TEntity> Find(Query<TEntity> query)
        {
            return query.Execute(CreateQuery<TEntity>(tableName));
        }

        public virtual void Add(TEntity entity)
        {
            //Contract.Requires(entity != null, "entity is null");

            AddObject(tableName, entity);
        }

        public virtual void Update(TEntity entity)
        {
            //Contract.Requires(entity != null, "entity is null");

            UpdateObject(entity);
        }

        public virtual void AttachEntity(TEntity entity)
        {
           // Contract.Requires(entity != null, "entity is null");

            AttachTo(tableName, entity);
        }

        public virtual void AttachEntityForUpsert(TEntity entity)
        {
            //Contract.Requires(entity != null, "entity is null");

            AttachTo(tableName, entity, null);
        }

        public virtual void DetachEntity(TEntity entity)
        {
           // Contract.Requires(entity != null, "entity is null");

            Detach(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            //Contract.Requires(entity != null, "entity is null");

            DeleteObject(entity);
        }

        public void SaveBatch()
        {
            SaveChangesWithRetries(SaveChangesOptions.Batch);
        }

        public void SaveAndReplaceOnUpdate()
        {
            SaveChangesWithRetries(SaveChangesOptions.ReplaceOnUpdate);
        }

        public void CreateTableIfNotExists()
        {
            cloudTableClient.CreateTableIfNotExist(tableName);
        }

        public void DeleteTable()
        {
            cloudTableClient.DeleteTableIfExist(tableName);
        }

        private static PagedResults<TEntity> CreatePagedResults(XmlSerializer serializer, ResultSegment<TEntity> response)
        {
            var pagedResults = new PagedResults<TEntity>();
            pagedResults.Results.AddRange(response.Results);
            pagedResults.ContinuationToken = response.ContinuationToken == null ? null :  ResultContinuationSerializer.SerializeToken(serializer, response.ContinuationToken);
            pagedResults.HasMoreResults = response.ContinuationToken != null;
            return pagedResults;
        }

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

        protected static CloudStorageAccount GetCloudStorageAccountByConfigurationSetting(string configurationSetting)
        {
            try
            {
                return CloudStorageAccount.FromConfigurationSetting(configurationSetting);
            }
            catch (Exception error)
            {
                throw new InvalidOperationException("Unable to find cloud storage account", error);
            }
        }
    }
}