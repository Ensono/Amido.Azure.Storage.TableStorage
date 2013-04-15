using System;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Tests.TableStorageRepositoryIntegrationTests
{
    [TestClass]
    public class FirstTests : TableStorageRepositoryTestBase
    {
        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_Exception_When_No_Data_Is_Returned()
        {
            Repository.First(new TableQuery<TestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "PartitionKey1")));
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException()
        {
            Repository.First(null);
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
            var result = Repository.First(new TableQuery<TestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "PartitionKey1")));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("RowKey0", result.RowKey);
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
            var result = Repository.First(new TableQuery<TestEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "PartitionKey1")));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("PartitionKey1", result.PartitionKey);
        }
    }
}