using System;
using System.Collections.Generic;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    /// <inheritdoc />
    /// <summary>
    /// Сравнение задач актёров по инициативе.
    /// </summary>
    public class TaskIniComparer: IComparer<IActorTask>
    {
        public int Compare(IActorTask x, IActorTask y)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            var xIni = x.Actor.Initiative;
            var yIni = y.Actor.Initiative;

            return xIni.CompareTo(yIni);
        }
    }
}
