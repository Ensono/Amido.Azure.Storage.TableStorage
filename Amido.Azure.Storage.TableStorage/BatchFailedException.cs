using System;
using System.Runtime.Serialization;

namespace Amido.Azure.Storage.TableStorage
{
    [Serializable]
    public class BatchFailedException : Exception
    {
        public bool IsConsistent { get; private set; }
        public Exception InnerCompensatingException { get; set; }

        public BatchFailedException()
        {
        }

        public BatchFailedException(string message)
            : base(message)
        {
        }

        public BatchFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public BatchFailedException(string message, bool isConsistent, Exception inner)
            : base(message, inner)
        {
            IsConsistent = isConsistent;
        }

        public BatchFailedException(string message, bool isConsistent, Exception innerBatchException, Exception innerCompensatingException)
            : base(message, innerBatchException)
        {
            IsConsistent = isConsistent;
            InnerCompensatingException = innerCompensatingException;
        }

        protected BatchFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}