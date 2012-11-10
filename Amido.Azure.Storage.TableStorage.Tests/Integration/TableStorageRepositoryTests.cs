using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Amido.Azure.Storage.TableStorage.Account;
using Amido.Azure.Storage.TableStorage.Queries;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class TableStorageRepositoryTests
    {
        public ITableStorageRepository<TestEntity> Repository; 

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            var process = Process.Start(@"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun", "/devstore");
            if (process != null)
            {
                process.WaitForExit();
            }
            else
            {
                throw new ApplicationException("Unable to start storage emulator.");
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            var accountConnection = new AccountConnection<TestEntity>(Properties.Resources.AccountConnectionString);

            Repository = new TableStorageRepository<TestEntity>(accountConnection);
            ((ITableStorageAdminRepository)Repository).CreateTableIfNotExists();
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            ((ITableStorageAdminRepository)Repository).DeleteTable();
        } 

        [TestClass]
        public class Query : TableStorageRepositoryTests
        {
            public class PagingTestQuery : Query<TestEntity>
            {
                public override IQueryable<TestEntity> Execute(IQueryable<TestEntity> query)
                {
                    return query.Take(5);
                }
            }

            [TestMethod]
            public void Should_Return_First_Five_Rows()
            {
                // Arrange
                for (var i = 0; i < 50; i++)
                {
                    Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
                    Repository.SaveBatch();
                }

                // Act
                var results = Repository.Query(new PagingTestQuery());

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
                    Repository.SaveBatch();
                }

                // Act
                var results = Repository.Query(new PagingTestQuery());

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
                    Repository.SaveBatch();
                }

                // Act
                var results = Repository.Query(new PagingTestQuery());

                // Assert
                Assert.IsNotNull(results);
                Assert.AreEqual(5, results.Results.Count);
                Assert.IsNotNull(results.ContinuationToken);
                Assert.IsTrue(results.ContinuationToken.Contains("ResultContinuation"));
                Assert.IsTrue(results.ContinuationToken.Contains("Table"));
                Assert.IsTrue(results.ContinuationToken.Contains("NextPartitionKey"));
                Assert.IsTrue(results.ContinuationToken.Contains("NextRowKey"));
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
                    Repository.SaveBatch();
                }

                var firstResults = Repository.Query(new PagingTestQuery());

                // Act
                var secondResults = Repository.Query(new PagingTestQuery {ContinuationTokenString = firstResults.ContinuationToken});

                // Assert
                Assert.IsNotNull(secondResults);
                Assert.AreEqual(5, secondResults.Results.Count);

                foreach (var secondResult in secondResults.Results)
                {
                    Assert.IsTrue(!firstResults.Results.Contains(secondResult));
                }
            }
        }

        [TestClass]
        public class FirstOrDefault : TableStorageRepositoryTests
        {
            [TestMethod]
            public void Should_Return_Null_As_Query_Returns_No_Data()
            {
                // Act
                var result = Repository.FirstOrDefault(new ListByPartitionKeyQuery<TestEntity>(Guid.NewGuid().ToString()));

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
                    Repository.SaveBatch();
                }

                // Act
                var result = Repository.FirstOrDefault(new ListByPartitionKeyQuery<TestEntity>("PartitionKey1"));

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("PartitionKey1", result.PartitionKey);
            }
        }

        [TestClass]
        public class First : TableStorageRepositoryTests
        {
            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void Should_Throw_Exception_When_No_Data_Is_Returned()
            {
                // Act - Assert
                Repository.First(new ListByPartitionKeyQuery<TestEntity>(Guid.NewGuid().ToString()));
            }

            [TestMethod]
            public void Should_Return_First_Row()
            {
                // Arrange
                for (var i = 0; i < 50; i++)
                {
                    Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
                    Repository.SaveBatch();
                }

                // Act
                var result = Repository.First(new ListByPartitionKeyQuery<TestEntity>("PartitionKey1"));

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("PartitionKey1", result.PartitionKey);
            }
        }

        [TestClass]
        public class GetByPartitionKeyAndRowKey : TableStorageRepositoryTests
        {
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
                    Repository.SaveBatch();
                }

                // Act
                var result = Repository.GetByPartitionKeyAndRowKey("PartitionKey1", "RowKey45");

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("PartitionKey1", result.PartitionKey);
                Assert.AreEqual("RowKey45", result.RowKey);
            }
        }

        [TestClass]
        public class ListByPartitionKey : TableStorageRepositoryTests
        {
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
                    Repository.SaveBatch();
                }

                // Act
                var result = Repository.ListByPartitionKey("PartitionKey8");

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(10, result.Results.Count);
                Assert.IsTrue(result.Results.All(x => x.PartitionKey == "PartitionKey8"));
            }
        }

        [TestClass]
        public class ListAll : TableStorageRepositoryTests
        {
            [TestMethod]
            public void Should_Return_All_Rows_In_Partition()
            {
                // Arrange
                for (var i = 0; i < 50; i++)
                {
                    Repository.Add(new TestEntity("PartitionKey1", "RowKey" + i));
                    Repository.SaveBatch();
                }

                // Act
                var results = Repository.ListAll();

                // Assert
                Assert.IsNotNull(results);
                Assert.AreEqual(50, results.Results.Count);
            }

            [TestMethod]
            public void Should_Return_All_Rows_Across_Partition_Keys()
            {
                // Arrange
                for (var i = 0; i < 50; i++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + i));
                    Repository.SaveBatch();
                }

                // Act
                var results = Repository.ListAll();

                // Assert
                Assert.IsNotNull(results);
                Assert.AreEqual(50, results.Results.Count);
            }

            [TestMethod]
            public void Should_Return_Continuation_Token_When_Item_Count_Greater_Than_1000()
            {
                // Arrange
                for (var i = 0; i < 100; i++)
                {
                    for (var j = 0; j < 20; j++)
                    {
                        Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                    }
                    Repository.SaveBatch();
                }

                // Act
                var results = Repository.ListAll();

                // Assert
                Assert.IsNotNull(results);
                Assert.AreEqual(1000, results.Results.Count);
                Assert.IsNotNull(results.ContinuationToken);
                Assert.IsTrue(results.ContinuationToken.Contains("ResultContinuation"));
                Assert.IsTrue(results.ContinuationToken.Contains("Table"));
                Assert.IsTrue(results.ContinuationToken.Contains("NextPartitionKey"));
                Assert.IsTrue(results.ContinuationToken.Contains("NextRowKey"));
            }

        }

        [TestClass]
        public class Find : TableStorageRepositoryTests
        {
            [TestMethod]
            public void Should_Return_Expected_Rows_From_Expression()
            {
                // Arrange
                for (var i = 0; i < 10; i++)
                {
                    for (var j = 0; j < 10; j++)
                    {
                        Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                    }
                    Repository.SaveBatch();
                }

                // Act
                var results = new List<TestEntity>(Repository.Find(x => x.PartitionKey == "PartitionKey2"));

                // Assert
                Assert.IsNotNull(results);
                Assert.AreEqual(10, results.Count);
            }

            [TestMethod]
            public void Should_Return_Expected_Rows_From_Query()
            {
                // Arrange
                for (var i = 0; i < 10; i++)
                {
                    for (var j = 0; j < 10; j++)
                    {
                        Repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                    }
                    Repository.SaveBatch();
                }

                // Act
                var results = new List<TestEntity>(Repository.Find(new ListByPartitionKeyQuery<TestEntity>("PartitionKey2")));

                // Assert
                Assert.IsNotNull(results);
                Assert.AreEqual(10, results.Count);
            }
        }
    }
}