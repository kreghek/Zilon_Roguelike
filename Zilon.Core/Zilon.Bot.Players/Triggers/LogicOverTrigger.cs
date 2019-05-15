using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public sealed class LogicOverTrigger : ILogicStateTrigger
    {
        public ILogicStateData CreateData(IActor actor)
        {
            return new EmptyLogicTriggerData();
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStateData data)
        {
            return currentState.Complete;
        }
    }
}
