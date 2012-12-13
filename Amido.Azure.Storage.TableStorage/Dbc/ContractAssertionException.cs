using System;
using System.Runtime.Serialization;

namespace Amido.Azure.Storage.TableStorage.Dbc
{
    /// <summary>
    /// Exception thrown when an assert contract check fails
    /// </summary>
    [Serializable]
    public class ContractAssertionException : Exception 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractAssertionException" /> class.
        /// </summary>
        public ContractAssertionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractAssertionException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ContractAssertionException(string message)
            : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractAssertionException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected ContractAssertionException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}