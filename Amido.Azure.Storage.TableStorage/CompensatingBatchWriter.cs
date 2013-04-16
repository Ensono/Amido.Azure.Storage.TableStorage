using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage
{
    public class CompensatingBatchWriter<TEntity> : ICompensatingBatchWriter<TEntity>
        where TEntity : class, ITableEntity, new()
    {
        private readonly BatchWriterHelper helper;
        private readonly ConcurrentQueue<TableEntityOperation> operations;

        internal CompensatingBatchWriter(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            helper = new BatchWriterHelper(cloudStorageAccount, tableName);
            operations = helper.Operations;
        }

        public void Insert(TEntity entity)
        {
        }

        public void Insert(IEnumerable<TEntity> entities)
        {
        }

        public void Execute()
        {
            //if success 
            //if compensated throw insert failed exception
            //if compensation fails throw inconsistednt state exception
        }
    }
}