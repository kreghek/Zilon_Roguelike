using System;
using System.Runtime.Serialization;

namespace Zilon.Core.Commands
{
    [Serializable]
    internal class CommandCantExecuteException : Exception
    {
        public CommandCantExecuteException()
        {
        }

        public CommandCantExecuteException(string message) : base(message)
        {
        }

        public CommandCantExecuteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CommandCantExecuteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}