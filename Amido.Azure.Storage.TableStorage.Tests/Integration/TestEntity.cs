using Microsoft.WindowsAzure.StorageClient;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    public class TestEntity : TableServiceEntity
    {
        public TestEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }
    }
}
