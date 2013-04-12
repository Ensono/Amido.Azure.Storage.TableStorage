using System;
using Amido.Azure.Storage.TableStorage.Dbc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amido.Azure.Storage.TableStorage.Tests.Dbc 
{
    [TestClass]
    public class Requires
    {
        [TestMethod]
        [ExpectedException(typeof(PreconditionException))]
        public void Should_Throw_PreConditionException() 
        {
            Contract.Requires(false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException() 
        {
            Contract.Requires<ArgumentException>(false);
        }

        [TestMethod]
        public void Should_Not_Throw_Exception() 
        {
            Contract.Requires(true);
        }

        [TestMethod]
        public void Should_Not_Throw_ArgumentException() 
        {
            Contract.Requires<ArgumentException>(true);
        }
    }

    [TestClass]
    public class Assert 
    {
        [TestMethod]
        [ExpectedException(typeof(ContractAssertionException))]
        public void Should_Throw_ContractAssertionException() 
        {
            Contract.Assert(false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException() 
        {
            Contract.Assert<ArgumentException>(false);
        }

        [TestMethod]
        public void Should_Not_Throw_Exception() 
        {
            Contract.Assert(true);
        }

        [TestMethod]
        public void Should_Not_Throw_ArgumentException() 
        {
            Contract.Assert<ArgumentException>(true);
        }
    }

    [TestClass]
    public class Ensures 
    {
        [TestMethod]
        [ExpectedException(typeof(PostconditionException))]
        public void Should_Throw_PostConditionException() 
        {
            Contract.Ensures(false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_ArgumentException()
        {
            Contract.Ensures<ArgumentException>(false);
        }

        [TestMethod]
        public void Should_Not_Throw_Exception() 
        {
            Contract.Ensures(true);
        }

        [TestMethod]
        public void Should_Not_Throw_ArgumentException()
        {
            Contract.Ensures<ArgumentException>(true);
        }
    }
}
