using System;
using Amido.Azure.Storage.TableStorage.Account;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration
{
    [TestClass]
    public class TableStorageRepositoryTests
    {
        [TestMethod]
        public void Should_Create_Table_Repository_From_Account_Connection()
        {
            var accountConnection = new AccountConnection<TestEntity>(Properties.Resources.AccountConnectionString);

            var repository = new TableStorageRepository<TestEntity>(accountConnection);

            Assert.IsNotNull(repository);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_On_Invalid_Connection_String()
        {
            var accountConnection = new AccountConnection<TestEntity>("Invalid");

            new TableStorageRepository<TestEntity>(accountConnection);
        }
    }
}
