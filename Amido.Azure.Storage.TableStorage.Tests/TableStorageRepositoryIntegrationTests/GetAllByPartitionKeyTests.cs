using System.Linq;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.TableStorageRepositoryIntegrationTests
{
    [TestClass]
    public class GetAllByPartitionKeyTests : TableStorageRepositoryTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException_If_PartitionKey_Null()
        {
            Repository.GetAllByPartitionKey(null);
        }

        [TestMethod]
        public void Should_Return_All_Entities_Accross_Partitions_When_Item_Count_Greater_Than_1000()
        {
            // Arrange
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    Repository.BatchWriter.Insert(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
            }
            for (var i = 10; i < 2100; i++)
            {
                Repository.BatchWriter.Insert(new TestEntity("PartitionKey1", "RowKey" + i));
            }
            Repository.BatchWriter.Execute();

            // Act
            var results = Repository.GetAllByPartitionKey("PartitionKey1").ToList();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2100, results.Count());
            Assert.AreEqual(2100, results.Count(x => x.PartitionKey == "PartitionKey1"));
        }
    }
}