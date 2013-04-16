using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests
{
    public class BatchWriterTestBase : TableStorageRepositoryTestBase
    {
        protected void AssertEntities(TestEntity[] results, int startEntity, int countEntities, int partitionNum, int startRowKey, string testValue1, string testValue2, int? testValue3)
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

        protected void InitializeData(int startPartitionNum, int numPartitions, int numRowsPerPartition)
        {
            for (var i = startPartitionNum; i < startPartitionNum + numPartitions; i++)
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

        protected IEnumerable<TestEntity> GetAllEntities()
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
