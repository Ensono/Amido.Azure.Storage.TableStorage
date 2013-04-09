using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class QueryTest2 : TableStorageRepositoryTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException()
        {
            Repository.Query(null);
        }

        [TestMethod]
        public void Should_Return_First_Five_Rows()
        {
            // Arrange
            for (var i = 0; i < 50; i++)
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
            for (var i = 0; i < 50; i++)
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
        public void Should_Return_Continuation_Token_When_More_Rows_Exist()
        {
            // Arrange
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            // Act
            var results = Repository.Query(new TableQuery<TestEntity>(), 5);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Results.Count);
            Assert.IsTrue(results.HasMoreResults);
            Assert.IsNotNull(results.ContinuationToken);
            //TODO: Some XDoc type validation of the continuationtoken
            //Assert.IsTrue(result.ContinuationToken.Contains("ResultContinuation"));
            //Assert.IsTrue(result.ContinuationToken.Contains("Table"));
            //Assert.IsTrue(result.ContinuationToken.Contains("NextPartitionKey"));
            //Assert.IsTrue(result.ContinuationToken.Contains("NextRowKey"));
        }

        [TestMethod]
        public void Should_Return_Next_Rows_Using_Valid_Continuation()
        {
            // Arrange
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            var firstResults = Repository.Query(new TableQuery<TestEntity>(), 5);

            // Assert
            Assert.IsNotNull(firstResults);

            // Act
            var secondResults = Repository.Query(new TableQuery<TestEntity>(), 5, firstResults.ContinuationToken);

            // Assert
            Assert.IsNotNull(secondResults);
            Assert.AreEqual(5, secondResults.Results.Count);

            foreach (var secondResult in secondResults.Results)
            {
                Assert.IsTrue(!firstResults.Results.Contains(secondResult));
            }
        }
    }
}