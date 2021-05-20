﻿using System.Threading;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface ISectorTaskSourceContext
    {
        /// <summary>
        /// External cancellation token to stop actor task calculation/waiting by task source.
        /// </summary>
        CancellationToken? CancellationToken { get; }

        ISector Sector { get; }
    }
}