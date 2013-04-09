using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class UpdateTests : TableStorageRepositoryTestBase 
    {
        //    [TestMethod]
        //    public void Should_Update_Entity_If_Present() 
        //    {
        //        // Arrange
        //        for(var i = 0; i < 1; i++) {
        //            for(var j = 0; j < 10; j++) {
        //                Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j) {TestValue = "Created"});
        //            }
        //            Repository.SaveBatch();
        //        }

        //        // Act
        //        var result = Repository.GetByPartitionKeyAndRowKey("PartitionKey0", "RowKey1");

        //        // Assert
        //        Assert.IsNotNull(result);
        //        Assert.AreEqual("PartitionKey0", result.PartitionKey);
        //        Assert.AreEqual("RowKey1", result.RowKey);

        //        var current = result.TestValue;

        //        // Act
        //        result.TestValue = "Updated";
        //        Repository.Update(result);
        //        Repository.SaveAndReplaceOnUpdate();

        //        result = Repository.GetByPartitionKeyAndRowKey("PartitionKey0", "RowKey1");

        //        // Assert
        //        Assert.IsNotNull(result);
        //        Assert.IsTrue(result.TestValue == "Updated");
        //        Assert.AreNotEqual(result.TestValue, current);
        //    }
    }
}