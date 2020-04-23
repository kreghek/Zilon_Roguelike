using System;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация селектора обработчиков действий.
    /// </summary>
    public sealed class ActUsageHandlerSelector : IActUsageHandlerSelector
    {
        private readonly IActUsageHandler[] _actUsageHandlers;

        public ActUsageHandlerSelector(IActUsageHandler[] actUsageHandlers)
        {
            _actUsageHandlers = actUsageHandlers;
        }

        /// <inheritdoc/>
        public IActUsageHandler GetHandler(IAttackTarget attackTarget)
        {
            if (attackTarget is null)
            {
                throw new ArgumentNullException(nameof(attackTarget));
            }

            foreach (var handler in _actUsageHandlers)
            {
                if (handler.TargetType.IsInstanceOfType(attackTarget))
                {
                    return handler;
                }
            }

            throw new HandlerNotFoundException($"Handler for type {attackTarget.GetType()} not found.");
        }
    }


    [Serializable]
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException() { }
        public HandlerNotFoundException(string message) : base(message) { }
        public HandlerNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected HandlerNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
