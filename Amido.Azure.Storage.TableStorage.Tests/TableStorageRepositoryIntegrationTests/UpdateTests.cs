using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.TableStorageRepositoryIntegrationTests
{
    [TestClass]
    public class UpdateTests : TableStorageRepositoryTestBase 
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
            result1.TestStringValue1 = "Updated";
            result1.TestStringValue2 = null;
            Repository.Update(result1);

            var result0 = Repository.GetByPartitionKeyAndRowKey("PartitionKey", "RowKey0");
            result1 = Repository.GetByPartitionKeyAndRowKey("PartitionKey", "RowKey1");

            // Assert
            Assert.IsNotNull(result0);
            Assert.IsTrue(result0.TestStringValue1 == "Created");
            Assert.IsTrue(result0.TestStringValue2 == "Created");
            Assert.AreEqual(result0.TestInt32Value, 1);
            Assert.IsNotNull(result1);
            Assert.IsTrue(result1.TestStringValue1 == "Updated");
            Assert.IsNull(result1.TestStringValue2);
            Assert.AreEqual(result1.TestInt32Value, 1);
        }
    }
}