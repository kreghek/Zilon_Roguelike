using System.Collections.Generic;

namespace Zilon.Bot.Players.Strategies
{
    public class LogicStateTree
    {
        public LogicStateTree()
        {
            Transitions = new Dictionary<ILogicState, IEnumerable<LogicTransition>>();
        }

        public ILogicState StartState { get; set; }

        public Dictionary<ILogicState, IEnumerable<LogicTransition>> Transitions { get; }
    }
}
