using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.SectorScene
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
        /// Выбранный узел.
        /// </summary>
        IMapNodeViewModel SelectedNode { get; set; }

        /// <summary>
        /// Пользовательский источник задач для актёров.
        /// </summary>
        HumanActorTaskSource TaskSource { get; set; }

        /// <summary>
        /// Текущий выбранный актёр на экране.
        /// </summary>
        IActorViewModel SelectedActor { get; set; }

        //TODO Объединить все объекты в секторе, которые можно выбирать, под единым интерфейсом.
        /// <summary>
        /// Текущий выбранный контейнер на экране.
        /// </summary>
        IContainerViewModel SelectedContainer { get; set; }
    }
}