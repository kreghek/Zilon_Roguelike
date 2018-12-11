using System;

namespace Assets.Zilon.Scripts.Services
{
    public interface ILogService
    {
        void Log(string message);

        event EventHandler<LogChangedEventArgs> LogChanged;
    }
}
