using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class InsertOrMergeTests : TableStorageRepositoryTestBase
    {
        [TestMethod]
        public void Should_Update_Entity_If_Present()
        {
            // Arrange
            for (var i = 0; i < 2; i++)
            {
                Repository.Add(new TestEntity("PartitionKey", "RowKey" + i) { TestStringValue1 = "Created", TestStringValue2 = "Created", TestInt32Value = 1 });
            }
            var result1 = Repository.GetByPartitionKeyAndRowKey("PartitionKey", "RowKey1");

            // Act
            result1.TestStringValue1 = "InsertedOrReplaced";
            result1.TestStringValue2 = null;
            Repository.InsertOrMerge(result1);
            Repository.InsertOrMerge(new TestEntity("PartitionKey", "RowKey2") { TestStringValue1 = "InsertedOrReplaced", TestInt32Value = 1 });

            var result0 = Repository.GetByPartitionKeyAndRowKey("PartitionKey", "RowKey0");
            result1 = Repository.GetByPartitionKeyAndRowKey("PartitionKey", "RowKey1");
            var result2 = Repository.GetByPartitionKeyAndRowKey("PartitionKey", "RowKey2");

            // Assert
            Assert.IsNotNull(result0);
            Assert.IsTrue(result0.TestStringValue1 == "Created");
            Assert.IsTrue(result0.TestStringValue2 == "Created");
            Assert.AreEqual(result0.TestInt32Value, 1);
            Assert.IsNotNull(result1);
            Assert.IsTrue(result1.TestStringValue1 == "InsertedOrReplaced");
            Assert.IsTrue(result1.TestStringValue2 == "Created");
            Assert.AreEqual(result1.TestInt32Value, 1);
            Assert.IsNotNull(result2);
            Assert.IsTrue(result1.TestStringValue1 == "InsertedOrReplaced");
            Assert.IsNull(result2.TestStringValue2);
            Assert.AreEqual(result1.TestInt32Value, 1);
        }
    }
}