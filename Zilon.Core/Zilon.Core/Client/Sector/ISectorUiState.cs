using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    /// <summary>
    ///     Состояние ввода игрока.
    /// </summary>
    /// <remarks>
    ///     Используется командами для получения ввода игрока. Хранит состояние объектов боя.
    /// </remarks>
    public interface ISectorUiState : IUiState
    {
        /// <summary>
        ///     Активный актёр.
        /// </summary>
        IActorViewModel ActiveActor { get; set; }

        ITacticalAct TacticalAct { get; set; }

        /// <summary>
        ///     Пользовательский источник задач для актёров.
        ///     Используется для упрощения доступа к источнику команд у текущего активного актёра.
        /// </summary>
        [Obsolete("Because we can gen TaskSource from ActiveActor")]
        IHumanActorTaskSource<ISectorTaskSourceContext> TaskSource { get; }

        /// <summary>
        ///     Выстреливает, когда изменяется активный персонаж игрока.
        ///     В первую очередь введено для того, чтобы инициировать визуализацию
        ///     UI-элементов, отображающих статус персонажа игрока.
        /// </summary>
        event EventHandler ActiveActorChanged;
    }
}