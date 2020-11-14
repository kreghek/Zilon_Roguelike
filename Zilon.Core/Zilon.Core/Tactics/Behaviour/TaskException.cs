namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Исключение, которые выбрасывается при нарушении выполнения задачи актёра.
    /// </summary>
    [Serializable]
    public sealed class TaskException : ApplicationException
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

        private TaskException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}