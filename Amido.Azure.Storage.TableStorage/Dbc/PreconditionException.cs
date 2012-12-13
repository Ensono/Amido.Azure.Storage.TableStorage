using System;
using System.Runtime.Serialization;

namespace Amido.Azure.Storage.TableStorage.Dbc
{
    /// <summary>
    /// Exception thrown when a precondition contract check fails
    /// </summary>
    [Serializable]
    public class PreconditionException : Exception 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreconditionException" /> class.
        /// </summary>
        public PreconditionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreconditionException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PreconditionException(string message)
            : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PreconditionException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected PreconditionException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}