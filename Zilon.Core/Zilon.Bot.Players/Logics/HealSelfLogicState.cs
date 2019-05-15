using System;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public class HealSelfLogicState : ILogicState
    {
        public bool Complete { get; }

        public ILogicStateData CreateData(IActor actor)
        {
            throw new NotImplementedException();
        }

        public IActorTask GetTask(IActor actor, ILogicStateData data)
        {
            throw new NotImplementedException();
        }
    }
}
