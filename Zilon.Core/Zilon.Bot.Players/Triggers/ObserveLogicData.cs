using System;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class ObserveLogicData: ILogicStateData
    {
        public ObserveLogicData(ILogicState logic)
        {
            Logic = logic ?? throw new ArgumentNullException(nameof(logic));
        }

        public ILogicState Logic { get; }
    }
}
