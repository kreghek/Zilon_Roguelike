using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Components;
using Zilon.Core.MapGenerators.StaticObjectFactories;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Specs.Steps
{
    [UsedImplicitly]
    [Binding]
    public sealed class MiningSteps : GenericStepsBase<CommonGameActionsContext>
    {
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
    }
}