using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class QueryTest : TableStorageRepositoryTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException()
        {
            Repository.Query(null);
        }

        [TestMethod]
        public void Should_Return_First_Five_Rows_In_Same_Partition()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
            }

            // Act
            var results = Repository.Query(new TableQuery<TestEntity>(), 5);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Results.Count);
        }

        [TestMethod]
        public void Should_Return_First_Five_Rows_Across_Partition_Keys()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + i));
            }

            // Act
            var results = Repository.Query(new TableQuery<TestEntity>(), 5);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Results.Count);
        }

        [TestMethod]
        public void Should_Return_All_Rows_Using_Valid_Continuation_Tokens()
        {
            // Arrange
            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            var firstResults = Repository.Query(new TableQuery<TestEntity>(), 5);

            // Assert
            Assert.IsNotNull(firstResults);
            Assert.AreEqual(5, firstResults.Results.Count);
            Assert.IsTrue(firstResults.HasMoreResults);
            Assert.IsNotNull(firstResults.ContinuationToken);

            // Act
            var secondResults = Repository.Query(new TableQuery<TestEntity>(), 5, firstResults.ContinuationToken);

            // Assert
            Assert.IsNotNull(secondResults);
            Assert.AreEqual(5, secondResults.Results.Count);
            Assert.IsTrue(secondResults.HasMoreResults);
            Assert.IsNotNull(secondResults.ContinuationToken);
            foreach (var secondResult in secondResults.Results)
            {
                Assert.IsFalse(firstResults.Results.Contains(secondResult));
            }

            // Act
            var thirdResults = Repository.Query(new TableQuery<TestEntity>(), 5, secondResults.ContinuationToken);

            // Assert
            Assert.IsNotNull(thirdResults);
            Assert.AreEqual(2, thirdResults.Results.Count);
            Assert.IsFalse(thirdResults.HasMoreResults);
            Assert.IsNull(thirdResults.ContinuationToken);
            foreach (var thirdResult in thirdResults.Results)
            {
                Assert.IsFalse(secondResults.Results.Contains(thirdResult));
            }
        }
    }
}