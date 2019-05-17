using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using LightInject;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Players.DevelopmentTests
{
    [TestFixture]
    public class BotActorTaskSourceTests
    {
        private ServiceContainer _serviceContainer;

        [Test]
        public async System.Threading.Tasks.Task GetActorTasksTestAsync()
        {
            var schemeService = _serviceContainer.GetInstance<ISchemeService>();
            var humanPlayer = _serviceContainer.GetInstance<HumanPlayer>();
            var gameLoop = _serviceContainer.GetInstance<IGameLoop>();
            var sectorManager = _serviceContainer.GetInstance<ISectorManager>();
            var scoreManager = _serviceContainer.GetInstance<IScoreManager>();
            var botActorTaskSource = _serviceContainer.GetInstance<IActorTaskSource>("bot");
            var monsterActorTaskSource = _serviceContainer.GetInstance<IActorTaskSource>("monster");
            var survivalRandomSource = _serviceContainer.GetInstance<ISurvivalRandomSource>();
            var propFactory = _serviceContainer.GetInstance<IPropFactory>();
            var actorManager = _serviceContainer.GetInstance<IActorManager>();

            await sectorManager.CreateSectorAsync();

            sectorManager.CurrentSector.ScoreManager = scoreManager;
            sectorManager.CurrentSector.HumanGroupExit += CurrentSector_HumanGroupExit;

            gameLoop.ActorTaskSources = new[] {
                botActorTaskSource,
                monsterActorTaskSource
            };

            var humanActor = CreateHumanActor(humanPlayer,
                schemeService,
                survivalRandomSource,
                propFactory,
                sectorManager,
                actorManager);

            while (!humanActor.Person.Survival.IsDead)
            {
                var humanPersonHp = humanActor.Person
                    .Survival
                    .Stats
                    .Single(x => x.Type == SurvivalStatType.Health)
                    .Value;

                gameLoop.Update();
            };

            Console.WriteLine($"Scores: {scoreManager.BaseScores}");
        }

        private void CurrentSector_HumanGroupExit(object sender, SectorExitEventArgs e)
        {
            Console.WriteLine("Выход");
        }

        [SetUp]
        public void StartUp()
        {
            _serviceContainer = new ServiceContainer();
            var startUp = new Startup();
            startUp.RegisterServices(_serviceContainer);
            startUp.ConfigureAux(_serviceContainer);
        }

        private static IActor CreateHumanActor(HumanPlayer humanPlayer,
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            ISectorManager sectorManager,
            IActorManager actorManager)
        {
            var personScheme = schemeService.GetScheme<IPersonScheme>("human-person");

            var playerActorStartNode = sectorManager.CurrentSector.Map.Regions
                .SingleOrDefault(x => x.IsStart).Nodes
                .First();

            if (humanPlayer.MainPerson == null)
            {
                var inventory = new Inventory();

                var evolutionData = new EvolutionData(schemeService);

                var defaultActScheme = schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

                var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, survivalRandomSource, inventory);

                humanPlayer.MainPerson = person;


                var classRoll = new Random().Next(1, 3);
                switch (classRoll)
                {
                    case 1:
                        AddEquipmentToActor(person.EquipmentCarrier, 2, "short-sword", schemeService, propFactory);
                        AddEquipmentToActor(person.EquipmentCarrier, 1, "steel-armor", schemeService, propFactory);
                        AddEquipmentToActor(person.EquipmentCarrier, 3, "wooden-shield", schemeService, propFactory);
                        break;

                    case 2:
                        AddEquipmentToActor(person.EquipmentCarrier, 2, "battle-axe", schemeService, propFactory);
                        AddEquipmentToActor(person.EquipmentCarrier, 3, "battle-axe", schemeService, propFactory);
                        AddEquipmentToActor(person.EquipmentCarrier, 0, "highlander-helmet", schemeService, propFactory);
                        break;

                    case 3:
                        AddEquipmentToActor(person.EquipmentCarrier, 2, "bow", schemeService, propFactory);
                        AddEquipmentToActor(person.EquipmentCarrier, 1, "leather-jacket", schemeService, propFactory);
                        AddEquipmentToActor(inventory, "short-sword", schemeService, propFactory);
                        AddResourceToActor(inventory, "arrow", 10, schemeService, propFactory);
                        break;

                    case 4:
                        AddEquipmentToActor(person.EquipmentCarrier, 2, "fireball-staff", schemeService, propFactory);
                        AddEquipmentToActor(person.EquipmentCarrier, 1, "scholar-robe", schemeService, propFactory);
                        AddEquipmentToActor(person.EquipmentCarrier, 0, "wizard-hat", schemeService, propFactory);
                        AddResourceToActor(inventory, "mana", 15, schemeService, propFactory);
                        break;

                    case 5:
                        AddEquipmentToActor(person.EquipmentCarrier, 2, "pistol", schemeService, propFactory);
                        AddEquipmentToActor(person.EquipmentCarrier, 0, "elder-hat", schemeService, propFactory);
                        AddResourceToActor(inventory, "bullet-45", 5, schemeService, propFactory);

                        AddResourceToActor(inventory, "packed-food", 1, schemeService, propFactory);
                        AddResourceToActor(inventory, "water-bottle", 1, schemeService, propFactory);
                        AddResourceToActor(inventory, "med-kit", 1, schemeService, propFactory);

                        AddResourceToActor(inventory, "mana", 5, schemeService, propFactory);
                        AddResourceToActor(inventory, "arrow", 3, schemeService, propFactory);
                        break;
                }

                AddResourceToActor(inventory, "packed-food", 1, schemeService, propFactory);
                AddResourceToActor(inventory, "water-bottle", 1, schemeService, propFactory);
                AddResourceToActor(inventory, "med-kit", 1, schemeService, propFactory);
            }

            var actor = new Actor(humanPlayer.MainPerson, humanPlayer, playerActorStartNode);

            actorManager.Add(actor);

            return actor;
        }

        private static void AddEquipmentToActor(IEquipmentCarrier equipmentCarrier, int slotIndex, string equipmentSid,
            ISchemeService schemeService, IPropFactory propFactory)
        {
            try
            {
                var equipmentScheme = schemeService.GetScheme<IPropScheme>(equipmentSid);
                var equipment = propFactory.CreateEquipment(equipmentScheme);
                equipmentCarrier[slotIndex] = equipment;
            }
            catch (KeyNotFoundException)
            {
                Debug.WriteLine($"Не найден объект {equipmentSid}");
            }
        }

        private static void AddEquipmentToActor(Inventory inventory, string equipmentSid,
            ISchemeService schemeService, IPropFactory propFactory)
        {
            try
            {
                var equipmentScheme = schemeService.GetScheme<IPropScheme>(equipmentSid);
                var equipment = propFactory.CreateEquipment(equipmentScheme);
                inventory.Add(equipment);
            }
            catch (KeyNotFoundException)
            {
                Debug.WriteLine($"Не найден объект {equipmentSid}");
            }
        }

        private static void AddResourceToActor(Inventory inventory, string resourceSid, int count,
            ISchemeService schemeService, IPropFactory propFactory)
        {
            try
            {
                var resourceScheme = schemeService.GetScheme<IPropScheme>(resourceSid);
                var resource = propFactory.CreateResource(resourceScheme, count);
                inventory.Add(resource);
            }
            catch (KeyNotFoundException)
            {
                Debug.WriteLine($"Не найден объект {resourceSid}");
            }
        }
    }
}