using System.Collections.Generic;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration 
{
    [TestClass]
    public class FindTests : TestBase 
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException_If_Query_Null() 
        {
            new List<TestEntity>(Repository.Find((TableQuery<TestEntity>)null));
        }

        //[TestMethod]
        //public void Should_Return_Expected_Rows_From_Expression() {
            
        //    // Act
        //    var results = new List<TestEntity>(Repository.Find(x => x.PartitionKey == "PartitionKey2"));

        //    // Assert
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(10, results.Count);
        //}

        //[TestMethod]
        //public void Should_Return_Expected_Rows_From_Query() {
           
        //    // Act
        //    var results = new List<TestEntity>(Repository.Find(new ListByPartitionKeyQuery<TestEntity>("PartitionKey2")));

        //    // Assert
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(10, results.Count);
        //}

        //[TestMethod]
        //public void Should_Not_Return_Rows_From_Query() {
            
        //    // Act
        //    var results = new List<TestEntity>(Repository.Find(new ListByPartitionKeyQuery<TestEntity>("PartitionKey25")));

        //    // Assert
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(0, results.Count);
        //}

        //[TestMethod]
        //public void Should_Return_Expected_Row_From_Query() {
        //    // Act
        //    var results = new List<TestEntity>(Repository.Find(new GetByPartitionKeyAndRowKeyQuery<TestEntity>("PartitionKey2", "RowKey2")));

        //    // Assert
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(1, results.Count);
        //}

        //[TestMethod]
        //public void Should_Not_Return_Row_From_Query() {
           
        //    // Act
        //    var results = new List<TestEntity>(Repository.Find(new GetByPartitionKeyAndRowKeyQuery<TestEntity>("PartitionKey2", "RowKey25")));

        //    // Assert
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(0, results.Count);
        //}

        //[TestMethod]
        //[ExpectedException(typeof(PreconditionException))]
        //public void Should_Throw_PreconditionException() {
           
        //    // Act
        //    var results = new List<TestEntity>(Repository.Find(new ListByPartitionKeyQuery<TestEntity>("PartitionKey2"), 0));

        //    // Assert
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(5, results.Count);
        //}

        //[TestMethod]
        //public void Should_Return_Expected_Number_Of_Rows_From_Query() {
            
        //    // Act
        //    var results = new List<TestEntity>(Repository.Find(new ListByPartitionKeyQuery<TestEntity>("PartitionKey2"), 5));

        //    // Assert
        //    Assert.IsNotNull(results);
        //    Assert.AreEqual(5, results.Count);
        //}
    }
}
