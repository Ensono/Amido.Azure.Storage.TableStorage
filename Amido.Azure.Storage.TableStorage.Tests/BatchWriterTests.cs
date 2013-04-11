using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;

namespace Amido.Azure.Storage.TableStorage.Tests
{
    [TestClass]
    public class BatchWriterTests : TableStorageRepositoryTestBase
    {
        private BatchWriter<TestEntity> batchWriter;

        [TestInitialize]
        public void SetUp()
        {
            batchWriter = Repository.BatchWriter;
        }

        [TestMethod]
        public void Should_Insert_Multiple_Batches_Accross_Partitions()
        {
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                {
                    batchWriter.Insert(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Created" });
                }
            }
            batchWriter.Execute();

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
                    batchWriter.Insert(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Created" });
                }
            }
            batchWriter.Execute();
        }

        [TestMethod]
        public void Should_Replace_Multiple_Batches_Accross_Partitions()
        {
            InitializeData(2, 120);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                {
                    batchWriter.Replace(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            batchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(240, results.Length);
            AssertEntities(results, 0, 120, 0, 0, "Updated", null, null);
            AssertEntities(results, 120, 120, 1, 0, "Updated", null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(StorageException))]
        public void Should_Fail_Replace_Batch_When_New_Entities_Included()
        {
            InitializeData(2, 20);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                {
                    batchWriter.Replace(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Created" });
                }
            }
            batchWriter.Execute();
        }

        [TestMethod]
        public void Should_Merge_Multiple_Batches_Accross_Partitions()
        {
            InitializeData(2, 120);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                {
                    batchWriter.Merge(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            batchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(240, results.Length);
            AssertEntities(results, 0, 120, 0, 0, "Updated", "Created2", 999);
            AssertEntities(results, 120, 120, 1, 0, "Updated", "Created2", 999);
        }

        [TestMethod]
        [ExpectedException(typeof(StorageException))]
        public void Should_Fail_Merge_Batch_When_New_Entities_Included()
        {
            InitializeData(2, 20);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 120; j++)
                {
                    batchWriter.Merge(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Created" });
                }
            }
            batchWriter.Execute();
        }

        [TestMethod]
        public void Should_InsertOrReplace_Multiple_Batches_Accross_Partitions()
        {
            InitializeData(2, 20);

            for (var i = 1; i < 3; i++)
            {
                for (var j = 10; j < 120; j++)
                {
                    batchWriter.InsertOrReplace(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            batchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(250, results.Length);
            AssertEntities(results, 0, 20, 0, 0, "Created1", "Created2", 999);
            AssertEntities(results, 20, 10, 1, 0, "Created1", "Created2", 999);
            AssertEntities(results, 30, 110, 1, 10, "Updated", null, null);
            AssertEntities(results, 140, 110, 2, 10, "Updated", null, null);
        }

        [TestMethod]
        public void Should_InsertOrMerge_Multiple_Batches_Accross_Partitions()
        {
            InitializeData(2, 20);

            for (var i = 1; i < 3; i++)
            {
                for (var j = 10; j < 120; j++)
                {
                    batchWriter.InsertOrMerge(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            batchWriter.Execute();

            var results = GetAllEntities().ToArray();

            Assert.AreEqual(250, results.Length);
            AssertEntities(results, 0, 20, 0, 0, "Created1", "Created2", 999);
            AssertEntities(results, 20, 10, 1, 0, "Created1", "Created2", 999);
            AssertEntities(results, 30, 10, 1, 10, "Updated", "Created2", 999);
            AssertEntities(results, 40, 100, 1, 20, "Updated", null, null);
            AssertEntities(results, 140, 110, 2, 10, "Updated", null, null);
        }

        [TestMethod]
        public void Should_Delete_Multiple_Batches_Accross_Partitions()
        {
            InitializeData(2, 120);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 110; j++)
                {
                    batchWriter.Delete(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            batchWriter.Execute();

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
                    batchWriter.Delete(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3")) { ETag = "*", TestStringValue1 = "Updated" });
                }
            }
            batchWriter.Execute();
        }

        private void AssertEntities(TestEntity[] results, int startEntity, int countEntities, int partitionNum, int startRowKey, string testValue1, string testValue2, int? testValue3)
        {
            for (var i = startEntity; i < startEntity + countEntities; i++)
            {
                Assert.AreEqual("PartitionKey" + partitionNum.ToString("D3"), results[i].PartitionKey, "On row " + 1);
                Assert.AreEqual("RowKey" + (startRowKey++).ToString("D3"), results[i].RowKey, "On row " + 1);
                Assert.AreEqual(testValue1, results[i].TestStringValue1, "On row " + 1);
                Assert.AreEqual(testValue2, results[i].TestStringValue2, "On row " + 1);
                Assert.AreEqual(testValue3, results[i].TestInt32Value, "On row " + 1);
            }
        }

        private void InitializeData(int numPartitions, int numRowsPerPartition)
        {
            for (var i = 0; i < numPartitions; i++)
            {
                for (var j = 0; j < numRowsPerPartition; j++)
                {
                    Repository.Add(new TestEntity("PartitionKey" + i.ToString("D3"), "RowKey" + j.ToString("D3"))
                    {
                        TestStringValue1 = "Created1",
                        TestStringValue2 = "Created2",
                        TestInt32Value = 999
                    });
                }
            }
        }

        private IEnumerable<TestEntity> GetAllEntities()
        {
            var result = Repository.ListAll();
            var results = result.Results.Select(x => x);
            while (result.HasMoreResults)
            {
                result = Repository.ListAll();
                results = results.Concat(result.Results);
            }
            return results;
        }
    }
}
