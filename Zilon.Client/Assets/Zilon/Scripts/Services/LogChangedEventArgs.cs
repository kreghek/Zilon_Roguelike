using System;

namespace Assets.Zilon.Scripts.Services
{
    public class LogChangedEventArgs: EventArgs
    {
        public LogChangedEventArgs(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Message { get; }
    }
}
