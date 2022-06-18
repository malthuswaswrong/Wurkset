using System.Runtime.Serialization;

namespace Wurkset
{
    [Serializable]
    internal class TooScaredException : Exception
    {
        public TooScaredException()
        {
        }

        public TooScaredException(string? message) : base(message)
        {
        }

        public TooScaredException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TooScaredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}