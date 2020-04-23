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

            throw new InvalidOperationException($"Handler for type {attackTarget.GetType()} not found.");
        }
    }
}
