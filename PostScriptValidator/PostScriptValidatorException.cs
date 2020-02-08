using System;
using System.Runtime.Serialization;

namespace PostScriptValidator
{
    [Serializable]
    internal class PostScriptValidatorException : Exception
    {
        public PostScriptValidatorException()
        {
        }

        public PostScriptValidatorException(string message) : base(message)
        {
        }

        public PostScriptValidatorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PostScriptValidatorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}