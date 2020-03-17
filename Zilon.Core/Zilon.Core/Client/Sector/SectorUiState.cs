using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    public class SectorUiState : UiStateBase, ISectorUiState
    {
        /// <summary>Активный актёр.</summary>
        public IActorViewModel ActiveActor { get; set; }

        /// <summary>Пользовательский источник задач для актёров.</summary>
        public IHumanActorTaskSource TaskSource { get; set; }

        /// <inheritdoc/>
        public ITacticalAct TacticalAct { get; set; }
    }
}
