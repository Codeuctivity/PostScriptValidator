using System;
using System.Runtime.Serialization;

namespace PostScriptValidator
{
    /// <summary>
    /// Gets thrown if calls to ghostscript failes
    /// </summary>
    [Serializable]
    public class PostScriptValidatorException : Exception
    {
        /// <summary>
        /// PostScriptValidatorException ctor
        /// </summary>
        public PostScriptValidatorException()
        {
        }
        /// <summary>
        /// PostScriptValidatorException ctor
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public PostScriptValidatorException(string message) : base(message)
        {
        }
        /// <summary>
        /// PostScriptValidatorException ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <returns></returns>
        public PostScriptValidatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
        /// <summary>
        /// PostScriptValidatorException ctor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected PostScriptValidatorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}