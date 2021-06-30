using System;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands.Sector
{
    public class SwitchToCombatModeCommand : ActorCommandBase
    {
        private readonly IPlayer _player;

        public SwitchToCombatModeCommand(ISectorUiState playerState, IPlayer player) : base(playerState)
        {
            _player = player;
        }

        public override CanExecuteCheckResult CanExecute()
        {
            return CanExecuteCheckResult.CreateSuccessful();
        }

        protected override void ExecuteTacticCommand()
        {
            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);

            var intention = new Intention<SwitchToCombatModeTask>(a =>
                new SwitchToCombatModeTask(a, taskContext));
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var taskSource = PlayerState.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(intention, actor);
        }
    }

    public class SwitchToIdleModeCommand : ActorCommandBase
    {
        private readonly IPlayer _player;

        public SwitchToIdleModeCommand(ISectorUiState playerState, IPlayer player) : base(playerState)
        {
            _player = player;
        }

        public override CanExecuteCheckResult CanExecute()
        {
            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var activeCombats = sector.ActiveCombats.Where(x => x.Participants.Contains(PlayerState.ActiveActor.Actor))
                .ToArray();
            if (activeCombats.Any())
            {
                return CanExecuteCheckResult.CreateFailed("There is combats with actor.");
            }

            return CanExecuteCheckResult.CreateSuccessful();
        }

        protected override void ExecuteTacticCommand()
        {
            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);

            var intention = new Intention<SwitchToIdleModeTask>(a =>
                new SwitchToIdleModeTask(a, taskContext));
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var taskSource = PlayerState.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(intention, actor);
        }
    }
}