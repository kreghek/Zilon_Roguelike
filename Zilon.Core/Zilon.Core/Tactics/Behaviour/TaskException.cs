using System;
using System.Runtime.Serialization;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Исключение, которые выбрасывается при нарушении выполнения задачи актёра.
    /// </summary>
    public class TaskException : ApplicationException
    {
        public TaskException()
        {
        }

        public TaskException(string message) : base(message)
        {
        }

        public TaskException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TaskException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
