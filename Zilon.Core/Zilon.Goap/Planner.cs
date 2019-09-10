using System.Collections.Generic;
using System.Linq;

namespace Zilon.Goap
{
    /// <summary>
    /// Планировщик, составляющий план достижения цели.
    /// </summary>
    public class Planner
    {
        /// <summary>
        /// Планирует набор действий для достижения указанной цели.
        /// </summary>
        /// <param name="goal">Цель.</param>
        /// <param name="availableActions">Доступные действия.</param>
        /// <returns>Возвращает набор действий в том порядке, в котором их необходимо выполнить для достижения цели.
        /// Или null, если цель не достижима через указанные действия.</returns>
        public IGoapAction[] Plan(IGoapGoal goal, IState currentState, IGoapAction[] availableActions)
        {
            var openActions = new Queue<IGoapAction>(availableActions);

            while (openActions.Any())
            {
                var action = 
            }
        }
    }
}
