using System;
using System.Collections.Generic;
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
            InitializeData(0, 2, 120);

            var testEntities = TestEntities();
            Repository.BatchWriter.Delete(testEntities);
            Repository.BatchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(20, results.Length);
            AssertEntities(results, 0, 10, 0, 110, "Created1", "Created2", 999);
            AssertEntities(results, 10, 10, 1, 110, "Created1", "Created2", 999);
        }

        [TestMethod]
        public void Should_Fail_Delete_Multiple_First_Batches_Accross_Partitions_When_New_Entities_Included()
        {
            InitializeData(1, 2, 110);

            var testEntities = TestEntities();
            Repository.BatchWriter.Delete(testEntities);
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
        public void Should_Fail_Delete_Multiple_Subsequent_Batches_Accross_Partitions_When_New_Entities_Included()
        {
            InitializeData(0, 2, 105);

            var testEntities = TestEntities();
            Repository.BatchWriter.Delete(testEntities);
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
                for (var j = 0; j < 110; j++)
                    testEntities.Add(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
            }
            return testEntities;
        }
    }
}
