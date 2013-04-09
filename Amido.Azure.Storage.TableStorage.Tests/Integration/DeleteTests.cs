using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class DeleteTests : TableStorageRepositoryTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException_If_Entity_Null()
        {
            Repository.Delete(null); ;
        }

        [TestMethod]
        public void Should_Delete_Entity_If_Present()
        {
            // Arrange
            for (var i = 0; i < 1; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            // Act
            var result = Repository.GetByPartitionKeyAndRowKey("PartitionKey0", "RowKey1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("PartitionKey0", result.PartitionKey);
            Assert.AreEqual("RowKey1", result.RowKey);

            // Act
            Repository.Delete(result);

            result = Repository.GetByPartitionKeyAndRowKey("PartitionKey0", "RowKey1");

            // Assert
            Assert.IsNull(result);
        }
    }
}