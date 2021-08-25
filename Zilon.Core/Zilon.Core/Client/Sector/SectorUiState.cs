using System;

using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    public class SectorUiState : UiStateBase, ISectorUiState
    {
        private readonly IAnimationBlockerService _animationBlockerService;
        private readonly ICommandLoopUpdater _commandLoopUpdater;
        private readonly ICommandPool _commandPool;
        private IActorViewModel? _activeActor;

        public SectorUiState(ICommandPool commandPool, IAnimationBlockerService animationBlockerService,
            ICommandLoopUpdater commandLoopUpdater)
        {
            _commandPool = commandPool;
            _animationBlockerService = animationBlockerService;
            _commandLoopUpdater = commandLoopUpdater;
        }

        /// <inheritdoc />
        public IActorViewModel? ActiveActor
        {
            get => _activeActor;
            set
            {
                _activeActor = value;
                ActiveActorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc />
        public IHumanActorTaskSource<ISectorTaskSourceContext>? TaskSource =>
            ActiveActor?.Actor?.TaskSource as IHumanActorTaskSource<ISectorTaskSourceContext>;

        public event EventHandler? ActiveActorChanged;

        /// <inheritdoc />
        public ICombatAct? TacticalAct { get; set; }

        /// <inheritdoc />
        public bool CanPlayerGivesCommand => _commandPool.IsEmpty && !_animationBlockerService.HasBlockers &&
                                             !_commandLoopUpdater.HasPendingCommands();
    }
}