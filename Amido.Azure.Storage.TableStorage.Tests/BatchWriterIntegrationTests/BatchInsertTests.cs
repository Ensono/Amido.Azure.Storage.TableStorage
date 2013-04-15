using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;

namespace Amido.Azure.Storage.TableStorage.Tests.BatchWriterIntegrationTests
{
    [TestClass]
    public class BatchInsertTests : BatchWriterTestBase
    {
        [TestMethod]
        public void Should_Insert_Multiple_Batches_Accross_Partitions()
        {
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                {
                    Repository.BatchWriter.Insert(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Created" });
                }
            }
            Repository.BatchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(240, results.Length);
            AssertEntities(results, 0, 120, 0, 0, "Created", null, null);
            AssertEntities(results, 120, 120, 1, 0, "Created", null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(StorageException))]
        public void Should_Fail_Insert_Batch_When_New_Entities_Included()
        {
            InitializeData(2, 20);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                {
                    Repository.BatchWriter.Insert(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Created" });
                }
            }
            Repository.BatchWriter.Execute();
        }
    }
}