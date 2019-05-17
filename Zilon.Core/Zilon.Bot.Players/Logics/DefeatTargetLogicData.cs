using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public sealed class DefeatTargetLogicData : ILogicStateData
    {
        public DefeatTargetLogicData(IAttackTarget target)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public IAttackTarget Target { get; }

        public int RefreshCounter { get; set; }

        public MoveTask MoveTask { get; set; }

        public IdleTask IdleTask { get; set; }
    }
}
