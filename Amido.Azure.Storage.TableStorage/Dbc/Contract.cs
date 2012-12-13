using System;
using System.Diagnostics;

namespace Amido.Azure.Storage.TableStorage.Dbc
{
    internal static class Contract {
        #region Requires

        public static void Requires(bool condition) 
        {
            Requires(condition, string.Empty);
        }

        public static void Requires(bool condition, string message) 
        {
            Requires<PreconditionException>(condition, message);
        }

        public static void Requires<TException>(bool condition) where TException : Exception 
        {
            Requires<TException>(condition, string.Empty);
        }

        public static void Requires<TException>(bool condition, string message) where TException : Exception 
        {
            ThrowException<TException>(typeof(TException), condition, message);
        }

        #endregion

        #region Assert

        public static void Assert(bool condition) 
        {
            Assert(condition, string.Empty);
        }

        public static void Assert(bool condition, string message) 
        {
            Assert<ContractAssertionException>(condition, message);
        }

        public static void Assert<TException>(bool condition) where TException : Exception 
        {
            Assert<TException>(condition, string.Empty);
        }

        public static void Assert<TException>(bool condition, string message) where TException : Exception 
        {
            ThrowException<TException>(typeof(TException), condition, message);
        }

        #endregion

        #region Ensures

        public static void Ensures(bool condition) 
        {
            Ensures(condition, string.Empty);
        }

        public static void Ensures(bool condition, string message) 
        {
            Ensures<PostconditionException>(condition, message);
        }

        public static void Ensures<TException>(bool condition) where TException : Exception 
        {
            Ensures<TException>(condition, string.Empty);
        }

        public static void Ensures<TException>(bool condition, string message) where TException : Exception 
        {
            ThrowException<TException>(typeof(TException), condition, message);
        }

        #endregion

        #region Helpers

        private static void ThrowException<TException>(Type exceptionType, bool condition, string message) where TException : Exception 
        {
            if(!condition) {
                if(!string.IsNullOrWhiteSpace(message)) {
                    Trace.TraceError(message);
                }
                throw (TException)Activator.CreateInstance(exceptionType, message);
            }
        }

        #endregion

    }
}