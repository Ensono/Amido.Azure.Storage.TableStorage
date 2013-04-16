using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public class CompensatingBatchWriter<TEntity> : ICompensatingBatchWriter<TEntity>
        where TEntity : class, ITableEntity, new()
    {
        private readonly TableStorageRepository<TEntity> tableStorageRepository;
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly string tableName;
        private readonly BatchWriterHelper helper;
        private int operationNumber;

        internal CompensatingBatchWriter(TableStorageRepository<TEntity> tableStorageRepository, CloudStorageAccount cloudStorageAccount, string tableName)
        {
            this.tableStorageRepository = tableStorageRepository;
            this.cloudStorageAccount = cloudStorageAccount;
            this.tableName = tableName;
            helper = InitializeHelper();
        }

        /// <summary>
        /// Insert entity into batch for execution.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        public void Insert(TEntity entity)
        {
            Contract.Requires(entity != null, "entity is null");

            helper.Operations.Enqueue(new TableEntityOperation(operationNumber++, entity, TableOperation.Insert(entity)));
        }

        /// <summary>
        /// Insert entities into batch for execution.
        /// </summary>
        /// <param name="entities">Entities to insert.</param>
        public void Insert(IEnumerable<TEntity> entities)
        {
            Contract.Requires(entities != null, "entities is null");

            foreach (var entity in entities) Insert(entity);
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
            catch (Exception batchException)
            {
                try
                {
                    CompensateForFailedBatch();
                }
                catch (Exception compensatingException)
                {
                    throw new BatchFailedException("An exception occurred while attempting to execute the batch. The compensating action also failed.", false, batchException, compensatingException);
                }
                throw new BatchFailedException("An exception occurred while attempting to execute the batch.", true, batchException);
            }
        }

        private void CompensateForFailedBatch()
        {
            var entities = helper.Operations.Select(x => x.Entity).Cast<TEntity>().ToArray();
            foreach (var entity in entities)
            {
                try
                {
                    tableStorageRepository.Delete(entity);
                }
                catch (StorageException ex)
                {
                    var webException = ex.InnerException as WebException;
                    if (webException == null || webException.Response == null || ((HttpWebResponse)webException.Response).StatusCode != HttpStatusCode.NotFound) 
                        throw;
                }
            }
        }

        private BatchWriterHelper InitializeHelper()
        {
            return new BatchWriterHelper(cloudStorageAccount, tableName);
        }
    }
}