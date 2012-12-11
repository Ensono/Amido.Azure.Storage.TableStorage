using System;
using System.Linq;
using System.Linq.Expressions;
using Amido.Azure.Storage.TableStorage.Paging;
using Amido.Azure.Storage.TableStorage.Queries;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage
{
    public interface ITableStorageRepository<TEntity> where TEntity : TableServiceEntity
    {
        void Add(TEntity entity);

        void Update(TEntity entity);

        void AttachEntity(TEntity entity);

        void AttachEntityForUpsert(TEntity entity);

        void DetachEntity(TEntity entity);

        void Delete(TEntity entity);

        void SaveBatch();

        void SaveAndReplaceOnUpdate();

        PagedResults<TEntity> Query(Query<TEntity> query);

        TEntity FirstOrDefault(Query<TEntity> query);

        TEntity First(Query<TEntity> query);

        TEntity GetByPartitionKeyAndRowKey(string partitionKey, string rowKey);

        PagedResults<TEntity> ListByPartitionKey(string partitionKey, string continuationToken = null);

        PagedResults<TEntity> ListByPartitionKey(string partitionKey, int resultPerPage, string continuationToken = null);

        PagedResults<TEntity> ListAll(string continuationToken = null);

        PagedResults<TEntity> ListAll(int resultPerPage, string continuationToken = null);

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression);

        IQueryable<TEntity> Find(Query<TEntity> query);
    }
}
