using System;
using System.Runtime.Serialization;

namespace Amido.Azure.Storage.TableStorage
{
    [Serializable]
    public class BatchFailedException : Exception
    {
        public bool IsConsistent { get; private set; }

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

        public BatchFailedException(bool isConsistent)
        {
            IsConsistent = isConsistent;
        }

        public BatchFailedException(string message, bool isConsistent)
            : base(message)
        {
            IsConsistent = isConsistent;
        }

        public BatchFailedException(string message, bool isConsistent, Exception inner)
            : base(message, inner)
        {
            IsConsistent = isConsistent;
        }

        protected BatchFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}