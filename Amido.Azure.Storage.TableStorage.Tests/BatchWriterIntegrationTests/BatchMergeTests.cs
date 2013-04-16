using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;

namespace Amido.Azure.Storage.TableStorage.Tests.BatchWriterIntegrationTests
{
    [TestClass]
    public class BatchMergeTests : BatchWriterTestBase
    {
        [TestMethod]
        public void Should_Merge_Multiple_Batches_Accross_Partitions()
        {
            InitializeData(0, 2, 120);

            var testEntities = TestEntities();
            Repository.BatchWriter.Merge(testEntities);
            Repository.BatchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(240, results.Length);
            AssertEntities(results, 0, 120, 0, 0, "Updated", "Created2", 999);
            AssertEntities(results, 120, 120, 1, 0, "Updated", "Created2", 999);
        }

        [TestMethod]
        public void Should_Fail_Merge_First_Batch_When_New_Entities_Included()
        {
            InitializeData(0, 2, 20);

            var testEntities = TestEntities();
            Repository.BatchWriter.Merge(testEntities);
            try
            {
                Repository.BatchWriter.Execute();
            }
            catch (BatchFailedException ex)
            {
                Assert.IsTrue(ex.IsConsistent);
                return;
            }
            Assert.Fail("Exception not thrown");
        }

        [TestMethod]
        public void Should_Fail_Merge_Subsequent_Batch_When_New_Entities_Included()
        {
            InitializeData(0, 2, 110);

            var testEntities = TestEntities();
            Repository.BatchWriter.Merge(testEntities);
            try
            {
                Repository.BatchWriter.Execute();
            }
            catch (BatchFailedException ex)
            {
                Assert.IsFalse(ex.IsConsistent);
                return;
            }
            Assert.Fail("Exception not thrown");
        }

        private static List<TestEntity> TestEntities()
        {
            var testEntities = new List<TestEntity>();
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                    testEntities.Add(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
            }
            return testEntities;
        }
    }
}