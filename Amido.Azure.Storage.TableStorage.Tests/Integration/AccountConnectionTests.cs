using System;
using Amido.Azure.Storage.TableStorage.Account;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Integration {
    [TestClass]
    public class AccountConnectionTests {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException_If_Connection_String_Null() 
        {
            var accountConnection = new AccountConnection<TestEntity>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreconditionException_If_Connection_String_Empty() 
        {
            var accountConnection = new AccountConnection<TestEntity>(string.Empty);
        }
    }
}
