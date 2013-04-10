using System;
using System.Linq;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.TableStorageRepository
{
    [TestClass]
    public class ListByPartitionKeyTests : TableStorageRepositoryTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException_If_PartiionKey_Null()
        {
            Repository.ListByPartitionKey(null);
        }

        [TestMethod]
        public void Should_Return_Empty_List_When_Partition_Key_Does_Not_Have_Rows()
        {
            // Act
            var result = Repository.ListByPartitionKey(Guid.NewGuid().ToString());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Results.Count);
        }

        [TestMethod]
        public void Should_Return_Valid_Rows()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            // Act
            var result = Repository.ListByPartitionKey("PartitionKey8");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Results.Count);
            Assert.IsFalse(result.HasMoreResults);
            Assert.IsTrue(string.IsNullOrEmpty(result.ContinuationToken));
            Assert.IsTrue(result.Results.All(x => x.PartitionKey == "PartitionKey8"));
        }

        [TestMethod]
        public void Should_Return_Valid_Rows_PagedResults()
        {
            // Arrange
            const string partiionKey = "PartitionKey0";
            const int resultPerPage = 20;

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            // Act
            var result = Repository.ListByPartitionKey(partiionKey, null, resultPerPage);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Results.Count);
            Assert.IsFalse(result.HasMoreResults);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.ContinuationToken));
            Assert.IsTrue(result.Results.All(x => x.PartitionKey == partiionKey));
        }

        [TestMethod]
        public void Should_Return_Valid_Rows_PagedResults_WithMore()
        {
            // Arrange
            const string partiionKey = "PartitionKey0";
            const int resultPerPage = 5;

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            // Act
            var result = Repository.ListByPartitionKey(partiionKey, null, resultPerPage);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(resultPerPage, result.Results.Count);
            Assert.IsTrue(result.HasMoreResults);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ContinuationToken));
            Assert.IsTrue(result.Results.All(x => x.PartitionKey == partiionKey));
        }
    }
}