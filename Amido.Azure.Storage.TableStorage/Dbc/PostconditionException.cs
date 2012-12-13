using System;
using System.Runtime.Serialization;

namespace Amido.Azure.Storage.TableStorage.Dbc
{
    /// <summary>
    /// Exception thrown when a postcondition contract check fails
    /// </summary>
    [Serializable]
    public class PostconditionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostconditionException" /> class.
        /// </summary>
        public PostconditionException()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PostconditionException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public PostconditionException(string message)
            : base(message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PostconditionException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected PostconditionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}