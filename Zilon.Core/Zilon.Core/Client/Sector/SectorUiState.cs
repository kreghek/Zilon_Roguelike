using System;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    public class SectorUiState : UiStateBase, ISectorUiState
    {
        private IActorViewModel _activeActor;

        /// <inheritdoc/>
        public IActorViewModel ActiveActor
        {
            get => _activeActor;
            set
            {
                _activeActor = value;
                ActiveActorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc/>
        public IHumanActorTaskSource<ISectorTaskSourceContext> TaskSource =>
            ActiveActor?.Actor?.TaskSource as IHumanActorTaskSource<ISectorTaskSourceContext>;

        public event EventHandler ActiveActorChanged;

        /// <inheritdoc/>
        public ITacticalAct TacticalAct { get; set; }
    }
}