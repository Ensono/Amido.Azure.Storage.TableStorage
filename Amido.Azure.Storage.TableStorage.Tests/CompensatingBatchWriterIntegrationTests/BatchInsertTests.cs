using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.CompensatingBatchWriterIntegrationTests
{
    [TestClass]
    public class BatchInsertTests : BatchWriterTestBase
    {
        [TestMethod]
        public void Should_Insert_Multiple_Batches_Accross_Partitions()
        {
            var testEntities = TestEntities();
            Repository.CompensatingBatchWriter.Insert(testEntities);
            Repository.CompensatingBatchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(240, results.Length);
            AssertEntities(results, 0, 120, 0, 0, "Created", null, null);
            AssertEntities(results, 120, 120, 1, 0, "Created", null, null);
        }

        [TestMethod]
        public void Should_Fail_Insert_First_Batch_When_New_Entities_Included()
        {
            InitializeData(0, 2, 20);
            var testEntities = TestEntities();
            Repository.CompensatingBatchWriter.Insert(testEntities);
            try
            {
                Repository.CompensatingBatchWriter.Execute();
            }
            catch (BatchFailedException ex)
            {
                Assert.IsTrue(ex.IsConsistent);
                return;
            }
            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        public void Should_Fail_Insert_Subsequent_Batch_When_New_Entities_Included()
        {
            InitializeData(1, 2, 20);
            var testEntities = TestEntities();
            Repository.CompensatingBatchWriter.Insert(testEntities);
            try
            {
                Repository.CompensatingBatchWriter.Execute();
            }
            catch (BatchFailedException ex)
            {
                Assert.IsTrue(ex.IsConsistent);
                return;
            }
            Assert.Fail("Exception not thrown");
        }

        private static IEnumerable<TestEntity> TestEntities()
        {
            var testEntities = new List<TestEntity>();
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                    testEntities.Add(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Created" });
            }
            return testEntities;
        }
    }
}