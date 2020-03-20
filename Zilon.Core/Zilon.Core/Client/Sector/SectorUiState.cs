using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    public class SectorUiState : UiStateBase, ISectorUiState
    {
        private IActorViewModel _activeActor;

        /// <summary>Активный актёр.</summary>
        public IActorViewModel ActiveActor
        {
            get => _activeActor;
            set
            {
                _activeActor = value;
                ActiveActorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Пользовательский источник задач для актёров.</summary>
        public IHumanActorTaskSource TaskSource { get; set; }

        public event EventHandler ActiveActorChanged;

        /// <inheritdoc/>
        public ITacticalAct TacticalAct { get; set; }
    }
}
