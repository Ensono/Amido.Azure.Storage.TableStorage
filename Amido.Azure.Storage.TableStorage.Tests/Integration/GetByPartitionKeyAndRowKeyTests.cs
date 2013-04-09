using System;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class GetByPartitionKeyAndRowKeyTests : TableStorageRepositoryTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException_If_PartionKey_Null()
        {
            Repository.GetByPartitionKeyAndRowKey(Guid.NewGuid().ToString(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException_If_RowKey_Null()
        {
            Repository.GetByPartitionKeyAndRowKey(null, Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void Should_Return_Null_As_Query_Returns_No_Data()
        {
            // Act
            var result = Repository.GetByPartitionKeyAndRowKey(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Should_Return_First_Row()
        {
            // Arrange
            for (var i = 0; i < 50; i++)
            {
                Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
            }

            // Act
            var result = Repository.GetByPartitionKeyAndRowKey("PartitionKey1", "RowKey45");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("PartitionKey1", result.PartitionKey);
            Assert.AreEqual("RowKey45", result.RowKey);
        }
    }
}