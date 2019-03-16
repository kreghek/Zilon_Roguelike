using System;
using System.Linq;

using FluentAssertions;

using LightInject;

using Moq;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
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

        [Given(@"Задаём броски для использования действий")]
        public void GivenЗадаёмБроскиДляИспользованияДействий()
        {
            switch (ScenarioContext.Current.ScenarioInfo.Title) {
                case "Успешный удар двумя оружиями.":
                    {
                        var dice = Context.Container.GetInstance<IDice>();

                        var actUsageRandomSourceMock = new Mock<TacticalActUsageRandomSource>(dice).As<ITacticalActUsageRandomSource>();
                        actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>()))
                            .Returns<Roll>(roll => roll.Dice / 2 * roll.Count);  // Всегда берётся среднее значение среди всех бросков
                        actUsageRandomSourceMock.Setup(x => x.RollToHit())
                            .Returns(4);
                        actUsageRandomSourceMock.Setup(x => x.RollArmorSave())
                            .Returns(4);
                        actUsageRandomSourceMock.Setup(x => x.RollUseSecondaryAct())
                            .Returns(6);
                        var actUsageRandomSource = actUsageRandomSourceMock.Object;

                        Context.SpecifyTacticalActUsageRandomSource(actUsageRandomSource);
                    }
                    break;

                case "Провальный удар двумя оружиями.":
                    {
                        var dice = Context.Container.GetInstance<IDice>();

                        var actUsageRandomSourceMock = new Mock<TacticalActUsageRandomSource>(dice).As<ITacticalActUsageRandomSource>();
                        actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>()))
                            .Returns<Roll>(roll => roll.Dice / 2 * roll.Count);  // Всегда берётся среднее значение среди всех бросков
                        actUsageRandomSourceMock.Setup(x => x.RollToHit())
                            .Returns(4);
                        actUsageRandomSourceMock.Setup(x => x.RollArmorSave())
                            .Returns(4);
                        actUsageRandomSourceMock.Setup(x => x.RollUseSecondaryAct())
                            .Returns(1);
                        var actUsageRandomSource = actUsageRandomSourceMock.Object;

                        Context.SpecifyTacticalActUsageRandomSource(actUsageRandomSource);
                    }
                    break;

                default:
                    throw new InvalidOperationException("Для этого сценарция не заданы броски кубов.");
            }
        }


        [When(@"Актёр игрока атакует монстра Id:(.*)")]
        public void WhenАктёрИгрокаАтакуетМонстраId(int monsterId)
        {
            var attackCommand = Context.Container.GetInstance<ICommand>("attack");
            var playerState = Context.Container.GetInstance<ISectorUiState>();

            var monster = Context.GetMonsterById(monsterId);

            var monsterViewModel = new TestActorViewModel
            {
                Actor = monster
            };

            playerState.SelectedViewModel = monsterViewModel;

            attackCommand.Execute();
        }

        [Then(@"Актёр игрока мертв")]
        public void ThenАктёрИгрокаМертв()
        {
            var actor = Context.GetActiveActor();

            actor.Person.Survival.IsDead.Should().BeTrue();
        }

        [Then(@"Монстр Id:(.*) успешно обороняется")]
        public void ThenМонстрIdУспешноОбороняется(int monsterId)
        {
            var visual = Context.VisualEvents.Last();

            visual.EventName.Should().Be(nameof(IActor.OnDefence));

            var monster = Context.GetMonsterById(monsterId);
            visual.Actor.Should().BeSameAs(monster);
        }

        [Then(@"Тактическое умение (.*) имеет дебафф на эффективность")]
        public void ThenТактическоеУмениеChopИмеетДебаффНаЭффективность(string tacticalActSid)
        {
            var actor = Context.GetActiveActor();

            var act = actor.Person.TacticalActCarrier.Acts.OfType<TacticalAct>()
                .Single(x => x.Scheme.Sid == tacticalActSid);

            act.Efficient.Modifiers.ResultBuff.Should().Be(-1);
        }

    }
}
