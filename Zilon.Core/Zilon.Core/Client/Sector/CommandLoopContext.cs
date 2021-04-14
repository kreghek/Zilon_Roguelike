﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client.Sector
{
    public sealed class CommandLoopContext : ICommandLoopContext
    {
        private readonly IHumanActorTaskSource<ISectorTaskSourceContext> _humanActorTaskSource;
        private readonly IPlayer _player;

        public CommandLoopContext(IPlayer player, IHumanActorTaskSource<ISectorTaskSourceContext> humanActorTaskSource)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _humanActorTaskSource =
                humanActorTaskSource ?? throw new ArgumentNullException(nameof(humanActorTaskSource));
        }

        public bool HasNextIteration
        {
            get
            {
                var mainPerson = _player.MainPerson;
                if (mainPerson is null)
                {
                    throw new InvalidOperationException("The main person is not defined to process commands.");
                }

                var playerPersonSurvivalModule = mainPerson.GetModule<ISurvivalModule>();

                return !playerPersonSurvivalModule.IsDead;
            }
        }

        public bool CanPlayerGiveCommand
        {
            get
            {
                var canIndentoToTaskSource = _humanActorTaskSource.CanIntent();
                var animationsAreComplete = true; // Implement this using IAnimationBlockerService.
                return canIndentoToTaskSource && animationsAreComplete;
            }
        }
    }
}