using System.Collections.Generic;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players
{
    public interface ILogicStrategyData
    {
        HashSet<IMapNode> ObserverdNodes { get; }
    }
}
