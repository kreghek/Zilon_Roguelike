using System;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.Logics
{
    public sealed class RoamingLogicState : ILogicState
    {
        public bool Complete { get; }

        public IActorTask GetCurrentTask()
        {
            throw new NotImplementedException();
        }
    }
}
