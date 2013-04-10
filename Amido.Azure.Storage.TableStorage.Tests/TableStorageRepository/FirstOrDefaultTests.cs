using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Tests.TableStorageRepository
{
    [TestClass]
    public class FirstOrDefaultTests : TableStorageRepositoryTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException()
        {
            Repository.FirstOrDefault(null);
        }

        [TestMethod]
        public void Should_Return_Null_As_Query_Returns_No_Data()
        {
            // Act
            var result = Repository.FirstOrDefault(new TableQuery<TestEntity>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, "5")));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Should_Return_First_Row()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
            }

            // Act
            var result = Repository.FirstOrDefault(new TableQuery<TestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "PartitionKey1")));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("PartitionKey1", result.PartitionKey);
        }

        [TestMethod]
        public void Should_Return_First_Row_Of_Paged_Results()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
            }

            // Act
            var result = Repository.FirstOrDefault(new TableQuery<TestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "PartitionKey1")));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("PartitionKey1", result.PartitionKey);
        }
    }
}