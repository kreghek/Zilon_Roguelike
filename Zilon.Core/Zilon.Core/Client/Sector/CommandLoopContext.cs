using System;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client.Sector
{
    public sealed class CommandLoopContext : ICommandLoopContext
    {
        private readonly IPlayer _player;
        private readonly IHumanActorTaskSource<ISectorTaskSourceContext> _humanActorTaskSource;

        public CommandLoopContext(IPlayer player, IHumanActorTaskSource<ISectorTaskSourceContext> humanActorTaskSource)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _humanActorTaskSource = humanActorTaskSource ?? throw new ArgumentNullException(nameof(humanActorTaskSource));
        }

        public bool HasNextIteration
        {
            get
            {
                var mainPerson = _player.MainPerson;
                if (mainPerson is null)
                {
                    throw new InvalidOperationException("Main person is not defined to process commands.");
                }

                var playerPersonSurvivalModule = mainPerson.GetModule<ISurvivalModule>();

                return !playerPersonSurvivalModule.IsDead;
            }
        }

        public Task WaitForGlobeUpdate(CancellationToken cancellationToken)
        {
            return Task.Run(async ()=> {
                while (true)
                {
                    if (_humanActorTaskSource.CanIntent())
                    {
                        break;
                    }

                    await Task.Delay(1);
                }
            }, cancellationToken);
        }
    }
}