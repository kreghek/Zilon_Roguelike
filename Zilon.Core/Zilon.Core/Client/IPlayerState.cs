using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    /// <summary>
    /// Состояние ввода игрока.
    /// </summary>
    /// <remarks>
    /// Используется командами для получения ввода игрока. Хранит состояние объектов боя.
    /// </remarks>
    public interface IPlayerState
    {
        /// <summary>
        /// Активный актёр.
        /// </summary>
        IActorViewModel ActiveActor { get; set; }

        /// <summary>
        /// Выбранный объект.
        /// </summary>
        ISelectableViewModel HoverViewModel { get; set; }

        /// <summary>
        /// Зафиксированный выбранный объект.
        /// </summary>
        ISelectableViewModel SelectedViewModel { get; set; }

        /// <summary>
        /// Пользовательский источник задач для актёров.
        /// </summary>
        IHumanActorTaskSource TaskSource { get; set; }
    }
}