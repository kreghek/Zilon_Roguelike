using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tests.Common;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Steps
{
    [UsedImplicitly]
    [Binding]
    public sealed class MiningSteps : GenericStepsBase<CommonGameActionsContext>
    {
        /// <summary>
        /// Количество миллисекунд, которые можно потратить на выполнение быстрой операции.
        /// Эта константа нужна, чтобы задавать лимит по времени. Чтобы быстрее проваливать тесты, которые "подвисают".
        /// </summary>
        private const int TEST_SHORT_OP_LIMIT_MS = 1000;

        public MiningSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Given(@"Есть руда Id:(\d+) в ячейке \((\d+), (\d+)\)")]
        [Given(@"the ore with id:(\d+) in the map node \((\d+), (\d+)\)")]
        public void GivenЕстьРудаIdВЯчейке(int staticObjectId, int coordX, int coordY)
        {
            var coords = new OffsetCoords(coordX, coordY);
            _ = Context.AddStaticObject(staticObjectId, PropContainerPurpose.OreDeposits, coords);
        }

        [Then(@"Объект Id:(.*) уничтожен")]
        public void ThenОбъектIdУничтожен(int id)
        {
            var sector = Context.Globe.SectorNodes.First().Sector;
            var staticObject = sector.StaticObjectManager.Items.SingleOrDefault(x => x.Id == id);

            staticObject.Should().BeNull();
        }

        [When(@"Актёр игрока атакует объект Id:(.*)")]
        [When(@"the player actor attacks object with id:(.*)")]
        public void WhenPlayerPersonAttacksObjectWithId(int targetId)
        {
            var attackCommand = Context.ServiceProvider.GetRequiredService<AttackCommand>();
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();

            var staticObject = Context.GetStaticObjectById(targetId);

            var monsterViewModel = new TestContainerViewModel
            {
                StaticObject = staticObject
            };

            playerState.SelectedViewModel = monsterViewModel;
            playerState.TacticalAct = GetUsedActs(playerState.ActiveActor.Actor).First();

            attackCommand.Execute();
        }

        [When(@"the player actor attacks object with id:(.*) while it exists")]
        public async Task WhenPlayerPersonAttacksObjectWithIdWhileItExistsAsync(int targetId)
        {
            var sector = Context.GetCurrentGlobeFirstSector();
            var staticObject = sector.StaticObjectManager.Items.SingleOrDefault(x => x.Id == targetId);

            while (staticObject != null)
            {
                WhenPlayerPersonAttacksObjectWithId(targetId);
                await WhenЯЖдуЕдиницВремениAsync(1).ConfigureAwait(false);

                staticObject = sector.StaticObjectManager.Items.SingleOrDefault(x => x.Id == targetId);
            }
        }

        private static IEnumerable<ITacticalAct> GetUsedActs(IActor actor)
        {
            if (actor.Person.GetModuleSafe<IEquipmentModule>() is null)
            {
                yield return actor.Person.GetModule<ICombatActModule>().CalcCombatActs().First();
            }
            else
            {
                var usedEquipmentActs = false;
                var slots = actor.Person.GetModule<IEquipmentModule>().Slots;
                for (var i = 0; i < slots.Length; i++)
                {
                    var slotEquipment = actor.Person.GetModule<IEquipmentModule>()[i];
                    if (slotEquipment == null)
                    {
                        continue;
                    }

                    if ((slots[i].Types & EquipmentSlotTypes.Hand) == 0)
                    {
                        continue;
                    }

                    var equipmentActs = from act in actor.Person.GetModule<ICombatActModule>().CalcCombatActs()
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
                    yield return actor.Person.GetModule<ICombatActModule>().CalcCombatActs().First();
                }
            }
        }

        public async Task WhenЯЖдуЕдиницВремениAsync(int timeUnitCount)
        {
            var globe = Context.Globe;
            var humatTaskSource = Context.ServiceProvider
                .GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();

            var counter = timeUnitCount;

            var survivalModule = playerState.ActiveActor?.Actor.Person?.GetModuleSafe<ISurvivalModule>();

            var isPlayerActor = playerState.ActiveActor?.Actor != null;
            if (isPlayerActor)
            {
                while (counter > 0)
                {
                    for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
                    {
                        if (humatTaskSource.CanIntent() && survivalModule?.IsDead == false)
                        {
                            WhenЯВыполняюПростой();
                        }

                        await globe.UpdateAsync().TimeoutAfter(TEST_SHORT_OP_LIMIT_MS).ConfigureAwait(false);
                    }

                    counter--;
                }
            }
            else
            {
                while (counter > 0)
                {
                    for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
                    {
                        await globe.UpdateAsync().TimeoutAfter(TEST_SHORT_OP_LIMIT_MS).ConfigureAwait(false);
                    }

                    counter--;
                }
            }
        }

        public void WhenЯВыполняюПростой()
        {
            var idleCommand = Context.ServiceProvider.GetRequiredService<NextTurnCommand>();
            idleCommand.Execute();
        }
    }
}