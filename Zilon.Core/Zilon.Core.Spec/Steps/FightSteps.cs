using System.Threading.Tasks;

using LightInject;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public sealed class FightSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public FightSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [When(@"Актёр игрока атакует монстра Id:(.*)")]
        public async Task WhenАктёрИгрокаАтакуетМонстраId(int monsterId)
        {
            var attackCommand = _context.Container.GetInstance<ICommand>("attack");
            var playerState = _context.Container.GetInstance<IPlayerState>();

            var monster = _context.GetMonsterById(monsterId);

            var monsterViewModel = new TestActorViewModel {
                Actor = monster
            };

            playerState.HoverViewModel = monsterViewModel;

            var asyncTask = playerState.TaskSource.GetActorTasksAsync(playerState.ActiveActor.Actor);

            attackCommand.Execute();


            var tasks = await asyncTask;
            foreach (var task in tasks)
            {
                task.Execute();
            }
        }

    }
}
