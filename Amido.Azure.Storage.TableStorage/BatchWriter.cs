using System;
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
        private int operationNumber;

        internal BatchWriter(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            helper = new BatchWriterHelper(cloudStorageAccount, tableName);
            operations = helper.Operations;
        }

        public void Insert(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Insert(entity)));
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                Insert(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Delete(entity)));
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public void InsertOrMerge(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.InsertOrMerge(entity)));
        }

        public void InsertOrMerge(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                InsertOrMerge(entity);
            }
        }

        public void InsertOrReplace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.InsertOrReplace(entity)));
        }

        public void InsertOrReplace(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                InsertOrReplace(entity);
            }
        }

        public void Merge(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Merge(entity)));
        }

        public void Merge(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                Merge(entity);
            }
        }

        public void Replace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Replace(entity)));
        }

        public void Replace(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                Replace(entity);
            }
        }

        public void Execute()
        {
            try
            {
                helper.Execute();
            }
            catch (Exception ex)
            {
                throw new BatchFailedException("An exception occurred while attempting to execute the batch.", helper.BatchesComitted == 0, ex);
            }
        }
    }
}