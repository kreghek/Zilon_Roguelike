using System;

namespace Zilon.Core.Client.Sector
{
    public sealed class ErrorOccuredEventArgs : EventArgs
    {
        public ErrorOccuredEventArgs(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public Exception Exception { get; }
    }
}
