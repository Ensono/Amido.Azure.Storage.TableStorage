using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;

namespace Amido.Azure.Storage.TableStorage.Tests.BatchWriterIntegrationTests
{
    [TestClass]
    public class BatchDeleteTests : BatchWriterTestBase
    {
        [TestMethod]
        public void Should_Delete_Multiple_Batches_Accross_Partitions()
        {
            InitializeData(2, 120);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 110; j++)
                {
                    Repository.BatchWriter.Delete(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            Repository.BatchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(20, results.Length);
            AssertEntities(results, 0, 10, 0, 110, "Created1", "Created2", 999);
            AssertEntities(results, 10, 10, 1, 110, "Created1", "Created2", 999);
        }

        [TestMethod]
        [ExpectedException(typeof(StorageException))]
        public void Should_Delete_Multiple_Batches_Accross_Partitions_When_New_Entities_Included()
        {
            InitializeData(2, 120);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 130; j++)
                {
                    Repository.BatchWriter.Delete(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            Repository.BatchWriter.Execute();
        }
    }
}
