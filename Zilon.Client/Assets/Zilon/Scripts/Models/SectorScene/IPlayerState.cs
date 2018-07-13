using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
    /// <summary>
    /// Состояние ввода игрока.
    /// </summary>
    /// <remarks>
    /// Испоьзуется командами для получения ввода игрока. Хранит состояние объектов боя.
    /// </remarks>
    internal interface IPlayerState
    {
        /// <summary>
        /// Активный актёр.
        /// </summary>
        ActorVM ActiveActor { get; set; }

        /// <summary>
        /// Выбранный узел.
        /// </summary>
        MapNodeVM SelectedNode { get; set; }

        /// <summary>
        /// Пользовательский источник задач для актёров.
        /// </summary>
        HumanActorTaskSource TaskSource { get; set; }

        /// <summary>
        /// Текущий выбранный актёр на экране.
        /// </summary>
        ActorVM SelectedActor { get; set; }
        
        //TODO Объединить все объекты в секторе, которые можно выбирать, под единым интерфейсом.
        /// <summary>
        /// Текущий выбранный контейнер на экране.
        /// </summary>
        ContainerVm SelectedContainer { get; set; }
    }
}