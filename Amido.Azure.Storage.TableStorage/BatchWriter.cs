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

        /// <summary>
        /// Insert entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public void Insert(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Insert(entity)));
        }

        /// <summary>
        /// Insert entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to insert.</param>
        public void Insert(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                Insert(entity);
            }
        }

        /// <summary>
        /// Delete entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        public void Delete(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Delete(entity)));
        }

        /// <summary>
        /// Delete entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        public void Delete(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        /// <summary>
        /// Insert or merge entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to insert or merge.</param>
        public void InsertOrMerge(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.InsertOrMerge(entity)));
        }

        /// <summary>
        /// Insert or merge entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to insert or merge.</param>
        public void InsertOrMerge(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                InsertOrMerge(entity);
            }
        }

        /// <summary>
        /// Insert or replace entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to insert or replace.</param>
        public void InsertOrReplace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.InsertOrReplace(entity)));
        }

        /// <summary>
        /// Insert or replace entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to insert or replace.</param>
        public void InsertOrReplace(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                InsertOrReplace(entity);
            }
        }

        /// <summary>
        /// Merge entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        public void Merge(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Merge(entity)));
        }

        /// <summary>
        /// Merge entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to merge.</param>
        public void Merge(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                Merge(entity);
            }
        }

        /// <summary>
        /// Replace entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to replace.</param>
        public void Replace(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Replace(entity)));
        }

        /// <summary>
        /// Replace entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to replace.</param>
        public void Replace(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities)
            {
                Replace(entity);
            }
        }

        /// <summary>
        /// Execute batch of operations by partition in order.
        /// </summary>
        /// <exception cref="BatchFailedException">Raised if the batch fails for any reason with IsConsisted property set to false if part of the batch has been committed.</exception>
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