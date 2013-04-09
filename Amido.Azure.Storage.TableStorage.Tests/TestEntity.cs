
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Tests
{
    public class TestEntity : TableEntity
    {
        public TestEntity()
        {
        }

        public TestEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public string TestStringValue { get; set; }
        public int TestInt32Value { get; set; }
    }
}
