using System;

namespace Zilon.Core.World
{
    /// <summary>
    /// Аргументы события, выстреливающего при случайной встрече при путешествиях на глобальной карте.
    /// </summary>
    public sealed class WildMeetingEventArgs : EventArgs
    {
        /// <summary>
        /// Узел глобальной карты, где произошло событие.
        /// </summary>
        public ProvinceNode MettingNode { get; }
    }
}
