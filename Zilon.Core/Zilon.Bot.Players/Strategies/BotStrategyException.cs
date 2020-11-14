using System;
using System.Collections.Generic;

namespace Zilon.Bot.Players.Strategies
{
    [Serializable]
    public class BotStrategyException : Exception
    {
        public BotStrategyException() { }

        public BotStrategyException(string message) : base(message) { }

        public BotStrategyException(string message, Exception inner) : base(message, inner) { }

        public BotStrategyException(IEnumerable<LogicStateTrack> selectionHistory)
        {
            SelectionHistory = selectionHistory;
        }

        protected BotStrategyException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public IEnumerable<LogicStateTrack> SelectionHistory { get; }
    }
}