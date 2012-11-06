using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class TableStorageRepositoryTests
    {
        private ITableStorageRepository<TestEntity> repository;

        [TestInitialize]
        public void Initialize()
        {
            var process = Process.Start(@"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun", "/devstore");
            if (process != null)
            {
                process.WaitForExit();
            }
            else
            {
                throw new ApplicationException("Unable to start storage emulator.");
            }

            repository = new TableStorageRepository<TestEntity>(Properties.Resources.AccountConnectionString);
            ((ITableStorageAdminRepository)repository).CreateTableIfNotExists();
        }

        [TestCleanup]
        public void Cleanup()
        {
            ((ITableStorageAdminRepository)repository).DeleteTable();
        }

        [TestMethod]
        public void Should_Return_Continuation_Token_When_Item_Count_Greater_Than_1000()
        {
            // Arrange
            for (var i = 0; i < 100; i++)
            {
                for (var j = 0; j < 20; j++)
                {
                    repository.Add(new TestEntity("PartitionKey" + i, "RowKey" + j));
                }
                repository.SaveBatch();
            }

            // Act
            var results = repository.ListAll();

            // Assert
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.ContinuationToken);
            Assert.IsTrue(results.ContinuationToken.Contains("ResultContinuation"));
            Assert.IsTrue(results.ContinuationToken.Contains("Table"));
            Assert.IsTrue(results.ContinuationToken.Contains("NextPartitionKey"));
            Assert.IsTrue(results.ContinuationToken.Contains("NextRowKey"));
        }
    }
}