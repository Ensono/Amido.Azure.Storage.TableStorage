using System;
using System.Diagnostics;
using Amido.Azure.Storage.TableStorage.Account;
using Amido.Azure.Storage.TableStorage.Tests.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests
{
    [TestClass]
    public class TableStorageRepositoryTestBase
    {
        protected ITableStorageRepository<TestEntity> Repository;

        [TestInitialize]
        public void Initialize()
        {
            var accountConnection = new AccountConnection<TestEntity>(Properties.Resources.AccountConnectionString);

            Repository = new TableStorageRepository<TestEntity>(accountConnection);
            ((ITableStorageAdminRepository)Repository).DeleteTable();
            ((ITableStorageAdminRepository)Repository).CreateTableIfNotExists();
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            ((ITableStorageAdminRepository)Repository).DeleteTable();
        }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
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
        }
    }
}