using System;

namespace Yaclip
{
    internal sealed class RunException : Exception
    {
        public RunException() : base()
        {
        }

        public RunException(string message) : base(message)
        {
        }

        public RunException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
