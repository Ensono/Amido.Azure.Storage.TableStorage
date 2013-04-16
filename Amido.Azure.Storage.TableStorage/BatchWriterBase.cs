using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public abstract class BatchWriterBase
    {
        protected readonly ConcurrentQueue<TableEntityOperation> Operations;

        private const int BatchSize = 100;
        private readonly string tableName;
        private readonly CloudTableClient cloudTableClient;

        protected BatchWriterBase(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            this.tableName = tableName;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            Operations = new ConcurrentQueue<TableEntityOperation>();
        }

        protected void DoExecute()
        {
            var partitionedOperations = Operations
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

        protected class TableEntityOperation
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
