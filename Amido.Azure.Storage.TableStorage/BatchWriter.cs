using System.Collections.Generic;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public class BatchWriter<TEntity> : BatchWriterBase, IBatchWriter<TEntity>
        where TEntity : class, ITableEntity, new()
    {
        internal BatchWriter(CloudStorageAccount cloudStorageAccount, string tableName)
            : base(cloudStorageAccount, tableName)
        {
        }

        public void Insert(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            Operations.Enqueue(new TableEntityOperation(entity, TableOperation.Insert(entity)));
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
        }

        public void Delete(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            Operations.Enqueue(new TableEntityOperation(entity, TableOperation.Delete(entity)));
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
        }

        public void InsertOrMerge(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            Operations.Enqueue(new TableEntityOperation(entity, TableOperation.InsertOrMerge(entity)));
        }

        public void InsertOrMerge(IEnumerable<TEntity> entities)
        {
        }

        public void InsertOrReplace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            Operations.Enqueue(new TableEntityOperation(entity, TableOperation.InsertOrReplace(entity)));
        }

        public void InsertOrReplace(IEnumerable<TEntity> entities)
        {
        }

        public void Merge(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            Operations.Enqueue(new TableEntityOperation(entity, TableOperation.Merge(entity)));
        }

        public void Merge(IEnumerable<TEntity> entities)
        {
        }

        public void Replace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            Operations.Enqueue(new TableEntityOperation(entity, TableOperation.Replace(entity)));
        }

        public void Replace(IEnumerable<TEntity> entities)
        {
        }

        public void Execute()
        {
            base.DoExecute();
        }
    }
}