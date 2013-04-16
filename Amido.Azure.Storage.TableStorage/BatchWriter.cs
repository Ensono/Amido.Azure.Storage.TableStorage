using System.Collections.Concurrent;
using System.Collections.Generic;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public class BatchWriter<TEntity> : IBatchWriter<TEntity>
        where TEntity : class, ITableEntity, new()
    {
        private readonly BatchWriterHelper helper;
        private readonly ConcurrentQueue<TableEntityOperation> operations;

        internal BatchWriter(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            helper = new BatchWriterHelper(cloudStorageAccount, tableName);
            operations = helper.Operations;
        }

        public void Insert(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(entity, TableOperation.Insert(entity)));
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
        }

        public void Delete(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(entity, TableOperation.Delete(entity)));
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
        }

        public void InsertOrMerge(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(entity, TableOperation.InsertOrMerge(entity)));
        }

        public void InsertOrMerge(IEnumerable<TEntity> entities)
        {
        }

        public void InsertOrReplace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(entity, TableOperation.InsertOrReplace(entity)));
        }

        public void InsertOrReplace(IEnumerable<TEntity> entities)
        {
        }

        public void Merge(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(entity, TableOperation.Merge(entity)));
        }

        public void Merge(IEnumerable<TEntity> entities)
        {
        }

        public void Replace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(entity, TableOperation.Replace(entity)));
        }

        public void Replace(IEnumerable<TEntity> entities)
        {
        }

        public void Execute()
        {
            helper.Execute();
        }
    }
}