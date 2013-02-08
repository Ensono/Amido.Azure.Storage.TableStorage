using System;
using System.Diagnostics;
using System.Linq;
using Amido.Azure.Storage.TableStorage.Account;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class QueryTests : TestBase
    {
        //readonly CloudTableClient DefaultTableClient = new CloudTableClient(new Uri(TargetTenantConfig.TableServiceEndpoint), TestBase.StorageCredentials);

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) 
        {
            var tableClient = GenerateCloudTableClient();
            CurrentTable = tableClient.GetTableReference(typeof(TestEntity).Name);
            CurrentTable.CreateIfNotExists();
            for(var i = 0; i < 20; i++) {
                var batch = new TableBatchOperation();
                for(var j = 0; j < 100; j++) {
                    var ent = new TestEntity("PartitionKey" + i, "RowKey" + j) { TestStringValue = "TestValue" + i, TestInt32Value = i + 1 };
                    batch.Insert(ent);
                }
                CurrentTable.ExecuteBatch(batch);
            }
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            CurrentTable.DeleteIfExists();
        }
        
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException() {
            // Act
            var results = Repository.Query(null);

            // Assert
            Assert.IsNull(results);
        }

        [TestMethod]
        public void Should_Return_First_Five_Rows() {
            
            // Act
            var results = Repository.Query(new TableQuery<TestEntity>(), 5);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(5, results.Results.Count);
            Assert.IsTrue(results.Results.All(x => x.PartitionKey.Equals("PartitionKey0")));
        }

        [TestMethod]
        public void Should_Return_First_Ten_Rows_ForPartionKey() {
            //Arrange
            var query = new TableQuery<TestEntity>();
            var condtion = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "PartitionKey3");
            
            // Act
            var results = Repository.Query(query.Where(condtion), 10);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(10, results.Results.Count);
            Assert.IsTrue(results.Results.All(x=>x.PartitionKey.Equals("PartitionKey3")));
        }

        [TestMethod]
        public void Should_Return_First_Ten_Rows_ForStringTestValue() 
        {
            //Arrange
            var query = new TableQuery<TestEntity>();
            var condtion = TableQuery.GenerateFilterCondition("TestStringValue", QueryComparisons.Equal, "TestValue5");
            
            // Act
            var results = Repository.Query(query.Where(condtion), 10);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(10, results.Results.Count);
            Assert.IsTrue(results.Results.All(x => x.TestStringValue.Equals("TestValue5")));
        }

        [TestMethod]
        public void Should_Return_First_Ten_Rows_ForStringIntValue() {
            //Arrange
            var query = new TableQuery<TestEntity>();
            var condtion = TableQuery.GenerateFilterConditionForInt("TestInt32Value", QueryComparisons.Equal, 7);

            // Act
            var results = Repository.Query(query.Where(condtion), 10);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(10, results.Results.Count);
            Assert.IsTrue(results.Results.All(x => x.TestInt32Value == 7));
        }

        [TestMethod]
        public void Should_Return_Continuation_Token_When_More_Rows_Exist() {
            // Arrange
            var query = new TableQuery<TestEntity>();

            // Act
            var results = Repository.Query(query, 100);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(100, results.Results.Count);
            Assert.IsNotNull(results.ContinuationToken);
            Assert.IsTrue(results.HasMoreResults);
            Assert.IsTrue(results.ContinuationToken.Contains("ContinuationToken"));
            Assert.IsTrue(results.ContinuationToken.Contains("<Type>Table</Type>"));
            Assert.IsTrue(results.ContinuationToken.Contains("NextPartitionKey"));
            Assert.IsTrue(results.ContinuationToken.Contains("NextRowKey"));
        }

        [TestMethod]
        public void Should_Return_Next_Rows_Using_Valid_Continuation() {
            // Arrange
            var query = new TableQuery<TestEntity>();

            // Act
            var firstResults = Repository.Query(query, 100);

            // Assert
            Assert.IsNotNull(firstResults);

            // Act
            var secondResults = Repository.Query(query, 100, firstResults.ContinuationToken);

            // Assert
            Assert.IsNotNull(secondResults);
            Assert.AreEqual(100, secondResults.Results.Count);

            foreach(var secondResult in secondResults.Results) 
            {
                Assert.IsTrue(!firstResults.Results.Contains(secondResult));
            }
        }

        [TestMethod]
        public void Should_Not_Return_ContinuationToken_On_Final_Call() {
            // Arrange
            var query = new TableQuery<TestEntity>();

            // Act
            var firstResults = Repository.Query(query);

            // Assert
            Assert.IsNotNull(firstResults);
            Assert.AreEqual(1000, firstResults.Results.Count);
            Assert.IsNotNull(firstResults.ContinuationToken);

            // Act
            var secondResults = Repository.Query(query, firstResults.ContinuationToken);

            // Assert
            Assert.IsNotNull(secondResults);
            Assert.AreEqual(1000, secondResults.Results.Count);
            Assert.IsNull(secondResults.ContinuationToken);  
        }
    }
}