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
            ((ITableStorageAdminRepository)Repository).CreateTableIfNotExists();
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            ((ITableStorageAdminRepository)Repository).DeleteTable();
        } 
    }
}