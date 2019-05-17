using System;

using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Logics
{
    public sealed class LootLogicData: ILogicStateData
    {
        public LootLogicData(IPropContainer propContainer)
        {
            PropContainer = propContainer;
        }

        public IPropContainer PropContainer { get; }
    }
}
