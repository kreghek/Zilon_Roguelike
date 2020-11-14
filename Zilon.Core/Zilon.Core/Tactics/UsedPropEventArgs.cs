using System;
using Zilon.Core.Props;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Класс для события.
    /// </summary>
    public class UsedPropEventArgs : EventArgs
    {
        public UsedPropEventArgs(IProp usedProp)
        {
            UsedProp = usedProp ?? throw new ArgumentNullException(nameof(usedProp));
        }

        public IProp UsedProp { get; }
    }
}