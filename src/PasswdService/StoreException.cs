using System;
using System.Diagnostics;

namespace PasswdService
{
    /// <summary>This exception is thrown by <see cref="IStore"/> when the requested data is unavailable.</summary>
    [DebuggerStepThrough]
    internal sealed class StoreException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="StoreException"/> class.</summary>
        public StoreException() :
            base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="StoreException"/> class.</summary>
        /// <param name="message">The message that describes the error.</param>
        public StoreException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="StoreException"/> class.</summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or <c>null</c> if no inner exception is
        ///     specified.
        /// </param>
        public StoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
