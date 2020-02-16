using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.ActorInteractionEvents;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Specs.Steps
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
            switch (ScenarioContext.Current.ScenarioInfo.Title)
            {
                case "Успешный удар двумя оружиями.":
                    {
                        var dice = Context.ServiceProvider.GetRequiredService<IDice>();

                        var actUsageRandomSourceMock = new Mock<TacticalActUsageRandomSource>(dice).As<ITacticalActUsageRandomSource>();
                        actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>()))
                            .Returns<Roll>(roll => roll.Dice / 2 * roll.Count);  // Всегда берётся среднее значение среди всех бросков
                        actUsageRandomSourceMock.Setup(x => x.RollToHit(It.IsAny<Roll>()))
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
                        var dice = Context.ServiceProvider.GetRequiredService<IDice>();

                        var actUsageRandomSourceMock = new Mock<TacticalActUsageRandomSource>(dice).As<ITacticalActUsageRandomSource>();
                        actUsageRandomSourceMock.Setup(x => x.RollEfficient(It.IsAny<Roll>()))
                            .Returns<Roll>(roll => roll.Dice / 2 * roll.Count);  // Всегда берётся среднее значение среди всех бросков
                        actUsageRandomSourceMock.Setup(x => x.RollToHit(It.IsAny<Roll>()))
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
            var attackCommand = Context.ServiceProvider.GetRequiredService<AttackCommand>();
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();

            var monster = Context.GetMonsterById(monsterId);

            var monsterViewModel = new TestActorViewModel
            {
                Actor = monster
            };

            playerState.SelectedViewModel = monsterViewModel;
            playerState.TacticalAct = GetUsedActs(monster).First();

            attackCommand.Execute();
        }

        private static IEnumerable<ITacticalAct> GetUsedActs(IActor actor)
        {
            if (actor.Person.EquipmentCarrier == null)
            {
                yield return actor.Person.TacticalActCarrier.Acts.First();
            }
            else
            {
                var usedEquipmentActs = false;
                var slots = actor.Person.EquipmentCarrier.Slots;
                for (var i = 0; i < slots.Length; i++)
                {
                    var slotEquipment = actor.Person.EquipmentCarrier[i];
                    if (slotEquipment == null)
                    {
                        continue;
                    }

                    if ((slots[i].Types & Components.EquipmentSlotTypes.Hand) == 0)
                    {
                        continue;
                    }

                    var equipmentActs = from act in actor.Person.TacticalActCarrier.Acts
                                        where act.Equipment == slotEquipment
                                        select act;

                    var usedAct = equipmentActs.FirstOrDefault();

                    if (usedAct != null)
                    {
                        usedEquipmentActs = true;

                        yield return usedAct;
                    }
                }

                if (!usedEquipmentActs)
                {
                    yield return actor.Person.TacticalActCarrier.Acts.First();
                }
            }
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
            var monster = Context.GetMonsterById(monsterId);

            // Проверяем наличие события успешной обороны.
            var monsterDodgeEvent = Context.RaisedActorInteractionEvents
                .OfType<DodgeActorInteractionEvent>()
                .SingleOrDefault(x => x.TargetActor == monster);

            monsterDodgeEvent.Should().NotBeNull();
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
