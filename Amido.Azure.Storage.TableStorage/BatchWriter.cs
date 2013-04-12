using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public class BatchWriter<TEntity> where TEntity : class, ITableEntity, new()
    {
        private const int BatchSize = 100;
        private readonly ConcurrentQueue<TableEntityOperation> operations;
        private readonly string tableName;
        private readonly CloudTableClient cloudTableClient;

        internal BatchWriter(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            this.tableName = tableName;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            operations = new ConcurrentQueue<TableEntityOperation>();
        }

        public void Insert(TEntity entity)
        {
            operations.Enqueue(new TableEntityOperation(entity, TableOperation.Insert(entity)));
        }

        public void Delete(TEntity entity)
        {
            operations.Enqueue(new TableEntityOperation(entity, TableOperation.Delete(entity)));
        }

        public void InsertOrMerge(TEntity entity)
        {
            operations.Enqueue(new TableEntityOperation(entity, TableOperation.InsertOrMerge(entity)));
        }

        public void InsertOrReplace(TEntity entity)
        {
            operations.Enqueue(new TableEntityOperation(entity, TableOperation.InsertOrReplace(entity)));
        }

        public void Merge(TEntity entity)
        {
            operations.Enqueue(new TableEntityOperation(entity, TableOperation.Merge(entity)));
        }

        public void Replace(TEntity entity)
        {
            operations.Enqueue(new TableEntityOperation(entity, TableOperation.Replace(entity)));
        }

        public void Execute() 
        {
            var partitionedOperations = operations
                .GroupBy(o => o.Entity.PartitionKey);

            foreach (var partitionedOperation in partitionedOperations)
            {
                var batch = 0;
                var batchOperation = GetOperations(partitionedOperation, batch);
                while (batchOperation.Any())
                {
                    var tableBatchOperation = MakeBatchOperation(batchOperation);

                    ExecuteBatchWithRetries(tableBatchOperation);

                    batch++;
                    batchOperation = GetOperations(partitionedOperation, batch);
                }
            }
        }

        private void ExecuteBatchWithRetries(TableBatchOperation tableBatchOperation) 
        {
            var tableRequestOptions = new TableRequestOptions { RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(2), 100) };

            var tableReference = cloudTableClient.GetTableReference(tableName);
            
            tableReference.ExecuteBatch(tableBatchOperation, tableRequestOptions);
        }

        private static TableBatchOperation MakeBatchOperation(IEnumerable<TableOperation> batchOperation) 
        {
            var tableBatchOperation = new TableBatchOperation();
            foreach (var operation in batchOperation)
            {
                tableBatchOperation.Add(operation);
            }
            return tableBatchOperation;
        }

        private static TableOperation[] GetOperations(IEnumerable<TableEntityOperation> operations, int batch)
        {
            return operations
                .Skip(batch * BatchSize)
                .Take(BatchSize)
                .Select(o => o.Operation)
                .ToArray();
        }

        private class TableEntityOperation
        {
            public TableEntityOperation(ITableEntity entity, TableOperation operation)
            {
                Entity = entity;
                Operation = operation;
            }

            public ITableEntity Entity { get; set; }
            public TableOperation Operation { get; set; }
        }
    }
}