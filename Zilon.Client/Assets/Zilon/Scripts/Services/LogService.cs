using System;

using UnityEngine;

namespace Assets.Zilon.Scripts.Services
{
    internal class LogService : ILogService
    {
        public event EventHandler<LogChangedEventArgs> LogChanged;

        public void Log(string message)
        {
            Debug.Log(message);
            LogChanged?.Invoke(this, new LogChangedEventArgs(message));
        }
    }
}
