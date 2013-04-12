using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.BatchWriter
{
    [TestClass]
    public class BatchInsertOrMergeTests : BatchWriterTestBase
    {
        [TestMethod]
        public void Should_InsertOrMerge_Multiple_Batches_Accross_Partitions()
        {
            InitializeData(2, 20);

            for (var i = 1; i < 3; i++)
            {
                for (var j = 10; j < 120; j++)
                {
                    Repository.BatchWriter.InsertOrMerge(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            Repository.BatchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(250, results.Length);
            AssertEntities(results, 0, 20, 0, 0, "Created1", "Created2", 999);
            AssertEntities(results, 20, 10, 1, 0, "Created1", "Created2", 999);
            AssertEntities(results, 30, 10, 1, 10, "Updated", "Created2", 999);
            AssertEntities(results, 40, 100, 1, 20, "Updated", null, null);
            AssertEntities(results, 140, 110, 2, 10, "Updated", null, null);
        }
    }
}