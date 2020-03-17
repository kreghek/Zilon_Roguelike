using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    /// <summary>
    /// Состояние ввода игрока.
    /// </summary>
    /// <remarks>
    /// Используется командами для получения ввода игрока. Хранит состояние объектов боя.
    /// </remarks>
    public interface ISectorUiState: IUiState
    {
        /// <summary>
        /// Активный актёр.
        /// </summary>
        IActorViewModel ActiveActor { get; set; }

        ITacticalAct TacticalAct { get; set; }

        /// <summary>
        /// Пользовательский источник задач для актёров.
        /// </summary>
        IHumanActorTaskSource TaskSource { get; set; }
    }
}