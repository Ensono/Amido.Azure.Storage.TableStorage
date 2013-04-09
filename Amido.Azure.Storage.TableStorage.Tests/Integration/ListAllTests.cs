using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class ListAllTests : TableStorageRepositoryTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))] // Which line throws the exception??
        public void Should_Throw_PreconditionException_Invalid_ResultPerPage()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
            }

            // Act
            var results = Repository.ListAll(0);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(50, results.Results.Count);
        }

        [TestMethod]
        public void Should_Return_All_Rows_In_Partition()
        {
            // Arrange
            for (var i = 0; i < 10; i++)
            {
                Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
            }

            // Act
            var results = Repository.ListAll();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(10, results.Results.Count);
        }

        [TestMethod]
        public void Should_Return_All_Rows_Across_Partition_Keys()
        {
            // Arrange
            const int resultCount = 10;
            for (var i = 0; i < resultCount; i++)
            {
                Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + i));
            }

            // Act
            var results = Repository.ListAll();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(resultCount, results.Results.Count);
            Assert.IsFalse(results.HasMoreResults);
            Assert.IsTrue(string.IsNullOrEmpty(results.ContinuationToken));
        }

        [TestMethod]
        public void Should_Return_Continuation_Token_When_Item_Count_Greater_Than_1000()
        {
            // Arrange
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 11; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            // Act
            var results = Repository.ListAll();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1000, results.Results.Count);
            Assert.IsNotNull(results.ContinuationToken);
            //TODO: Some XDoc type validation of the continuationtoken
            //Assert.IsTrue(result.ContinuationToken.Contains("ResultContinuation"));
            //Assert.IsTrue(result.ContinuationToken.Contains("Table"));
            //Assert.IsTrue(result.ContinuationToken.Contains("NextPartitionKey"));
            //Assert.IsTrue(result.ContinuationToken.Contains("NextRowKey"));
        }

        [TestMethod]
        public void Should_Return_Continuation_Token_PagedResults()
        {
            // Arrange
            const int resultPerPage = 2;

            // Arrange
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            // Act
            var result = Repository.ListAll(resultPerPage);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(resultPerPage, result.Results.Count);
            Assert.IsTrue(result.HasMoreResults);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ContinuationToken));

            //TODO: Some XDoc type validation of the continuationtoken
            //Assert.IsTrue(result.ContinuationToken.Contains("ResultContinuation"));
            //Assert.IsTrue(result.ContinuationToken.Contains("Table"));
            //Assert.IsTrue(result.ContinuationToken.Contains("NextPartitionKey"));
            //Assert.IsTrue(result.ContinuationToken.Contains("NextRowKey"));
        }

        [TestMethod]
        public void Should_Not_Return_Continuation_Token_PagedResults_SecondCall()
        {
            // Arrange
            const int resultPerPage = 4;

            // Arrange
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }

            // Act
            var result = Repository.ListAll(resultPerPage);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(resultPerPage, result.Results.Count);
            Assert.IsTrue(result.HasMoreResults);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ContinuationToken));
            //TODO: Some XDoc type validation of the continuationtoken
            //Assert.IsTrue(result.ContinuationToken.Contains("ResultContinuation"));
            //Assert.IsTrue(result.ContinuationToken.Contains("Table"));
            //Assert.IsTrue(result.ContinuationToken.Contains("NextPartitionKey"));
            //Assert.IsTrue(result.ContinuationToken.Contains("NextRowKey"));

            // Act
            result = Repository.ListAll(resultPerPage, result.ContinuationToken);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(resultPerPage, result.Results.Count);
            Assert.IsFalse(result.HasMoreResults);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.ContinuationToken));
        }
    }
}