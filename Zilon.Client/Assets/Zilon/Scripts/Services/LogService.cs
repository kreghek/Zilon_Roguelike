using System;

namespace Assets.Zilon.Scripts.Services
{
    internal class LogService : ILogService
    {
        public event EventHandler<LogChangedEventArgs> LogChanged;

        public void Log(string message)
        {
            LogChanged?.Invoke(this, new LogChangedEventArgs(message));
        }
    }
}
