using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using LightInject;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Zilon.Bot.Sdk;
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
        private Startup _startUp;
        private Scope _sectorServiceContainer;
        private bool _changeSector;

        [Test]
        [TestCase("joe")]
        [TestCase("duncan")]
        [TestCase("")]
        [TestCase("monster")]
        public async Task GetActorTasksTestAsync(string mode)
        {
            var _globalServiceContainer = new ServiceCollection();
            var startUp = new Startup();
            var startUp.RegisterServices(_globalServiceContainer);

            //TODO Объяснить, почему тут нужно использовать ConfigureAwait(false)
            // Это рекомендация Codacy.
            // Но есть статья https://habr.com/ru/company/clrium/blog/463587/,
            // в которой объясняется, что не всё так просто.
            // Нужно чёткое понимание, зачем здесь ConfigureAwait(false) и
            // к какому результату это приводит по сравнению с простым await.
            var humanActor = await CreateSectorAsync().ConfigureAwait(false);

            var gameLoop = _sectorServiceContainer.GetInstance<IGameLoop>();
            var scoreManager = _globalServiceContainer.GetInstance<IScoreManager>();

            var botActorTaskSource = _sectorServiceContainer.GetInstance<ISectorActorTaskSource>("bot");
            botActorTaskSource.Configure(new BotSettings { Mode = mode });

            while (!humanActor.Person.Survival.IsDead)
            {
                gameLoop.Update();

                if (_changeSector)
                {
                    //TODO Объяснить, почему тут нужно использовать ConfigureAwait(false)
                    humanActor = await CreateSectorAsync().ConfigureAwait(false);

                    gameLoop = _sectorServiceContainer.GetInstance<IGameLoop>();
                    botActorTaskSource = _sectorServiceContainer.GetInstance<ISectorActorTaskSource>("bot");
                    botActorTaskSource.Configure(new BotSettings { Mode = mode });

                    _changeSector = false;
                }
            };

            Console.WriteLine($"Scores: {scoreManager.BaseScores}");
        }

        private void CurrentSector_HumanGroupExit(object sender, SectorExitEventArgs e)
        {
            Console.WriteLine("Exit");
            _changeSector = true;

            var sectorManager = _sectorServiceContainer.GetInstance<ISectorManager>();
            sectorManager.CurrentSector.HumanGroupExit -= CurrentSector_HumanGroupExit;
        }

        private async Task<IActor> CreateSectorAsync()
        {
            if (_sectorServiceContainer != null && !_sectorServiceContainer.IsDisposed)
            {
                _sectorServiceContainer.Dispose();
            }

            _sectorServiceContainer = _globalServiceContainer.BeginScope();

            _startUp.ConfigureAux(_sectorServiceContainer);

            var schemeService = _globalServiceContainer.GetInstance<ISchemeService>();
            var humanPlayer = _globalServiceContainer.GetInstance<HumanPlayer>();
            var survivalRandomSource = _globalServiceContainer.GetInstance<ISurvivalRandomSource>();
            var propFactory = _globalServiceContainer.GetInstance<IPropFactory>();
            var scoreManager = _globalServiceContainer.GetInstance<IScoreManager>();

            var gameLoop = _sectorServiceContainer.GetInstance<IGameLoop>();
            var sectorManager = _sectorServiceContainer.GetInstance<ISectorManager>();
            var botActorTaskSource = _sectorServiceContainer.GetInstance<IActorTaskSource>("bot");
            var actorManager = _sectorServiceContainer.GetInstance<IActorManager>();
            var monsterActorTaskSource = _sectorServiceContainer.GetInstance<IActorTaskSource>("monster");

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

            return humanActor;
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

                // TODO Использовать генератор персонажа, как в игре.
                // Для этого нужно научить ботов корректно использовать оружие дальнего боя и посохи лечения.
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

                    default:
                        throw new InvalidOperationException("Эта комбинация начальной экипировки не поддерживается.");
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