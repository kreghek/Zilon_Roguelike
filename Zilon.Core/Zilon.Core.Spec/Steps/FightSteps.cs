using System.Linq;
using FluentAssertions;

using LightInject;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tactics;
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
        public void WhenАктёрИгрокаАтакуетМонстраId(int monsterId)
        {
            var attackCommand = _context.Container.GetInstance<ICommand>("attack");
            var playerState = _context.Container.GetInstance<IPlayerState>();

            var monster = _context.GetMonsterById(monsterId);

            var monsterViewModel = new TestActorViewModel
            {
                Actor = monster
            };

            playerState.HoverViewModel = monsterViewModel;

            attackCommand.Execute();
        }

        [Then(@"Актёр игрока мертв")]
        public void ThenАктёрИгрокаМертв()
        {
            var actor = _context.GetActiveActor();

            actor.Person.Survival.IsDead.Should().BeTrue();
        }

        [Then(@"Монстр Id:(.*) успешно обороняется")]
        public void ThenМонстрIdУспешноОбороняется(int monsterId)
        {
            var visual = _context.VisualEvents.Last();

            visual.EventName.Should().Be(nameof(IActor.OnDefence));

            var monster = _context.GetMonsterById(monsterId);
            visual.Actor.Should().BeSameAs(monster);
        }
    }
}
