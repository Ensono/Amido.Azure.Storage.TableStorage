using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    internal class BatchWriterHelper
    {
        private const int BatchSize = 100;
        private readonly string tableName;
        private readonly CloudTableClient cloudTableClient;

        public BatchWriterHelper(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            this.tableName = tableName;
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            Operations = new ConcurrentQueue<TableEntityOperation>();
        }

        public ConcurrentQueue<TableEntityOperation> Operations { get; private set; }
        public int BatchesComitted { get; private set; }

        public void Execute()
        {
            var partitionedOperations = Operations
                .GroupBy(o => o.Entity.PartitionKey)
                .OrderBy(o => o.Key);

            foreach (var partitionedOperation in partitionedOperations)
            {
                var batch = 0;
                var batchOperation = GetOperations(partitionedOperation, batch);
                while (batchOperation.Any())
                {
                    var tableBatchOperation = MakeBatchOperation(batchOperation);

                    ExecuteBatchWithRetries(tableBatchOperation);

                    batch++;
                    BatchesComitted++;
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
                .OrderBy(o => o.OperationNumber)
                .Skip(batch * BatchSize)
                .Take(BatchSize)
                .Select(o => o.Operation)
                .ToArray();
        }
    }
}
