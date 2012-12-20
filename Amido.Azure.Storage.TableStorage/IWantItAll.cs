using System.Collections.Generic;
using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage
{
    public interface IWantItAll<TEntity> where TEntity : TableServiceEntity
    {
        List<TEntity> ListAllByPartitionKey(string partitionKey);
    }
}
