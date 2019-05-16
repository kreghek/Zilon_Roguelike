using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using LightInject;

using Zilon.Bot.Sdk;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot
{
    class Program
    {
        private const string SERVER_RUN_ARG = "ServerRun";
        private const string SCORE_PREFFIX_ARG = "ScorePreffix";
        private const string SCORE_FILE_PATH = "bot-scores";

        static async Task Main(string[] args)
        {
            var tacticContainer = new ServiceContainer();
            var startUp = new Startup();
            startUp.ConfigureTacticServices(tacticContainer);

            LoadBotAssembly("cdt", "Zilon.Bot.Players.LightInject.dll", tacticContainer, tacticContainer);

            var schemeService = tacticContainer.GetInstance<ISchemeService>();
            var humanPlayer = tacticContainer.GetInstance<HumanPlayer>();
            var gameLoop = tacticContainer.GetInstance<IGameLoop>();
            var sectorManager = tacticContainer.GetInstance<ISectorManager>();
            var scoreManager = tacticContainer.GetInstance<IScoreManager>();
            var botActorTaskSource = tacticContainer.GetInstance<IActorTaskSource>("bot");
            var monsterActorTaskSource = tacticContainer.GetInstance<IActorTaskSource>("monster");
            var survivalRandomSource = tacticContainer.GetInstance<ISurvivalRandomSource>();
            var propFactory = tacticContainer.GetInstance<IPropFactory>();
            var actorManager = tacticContainer.GetInstance<IActorManager>();

            await sectorManager.CreateSectorAsync();

            sectorManager.CurrentSector.ScoreManager = scoreManager;

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

            foreach (var actor in actorManager.Items)
            {
                actor.Moved += Actor_Moved;
                actor.UsedAct += Actor_UsedAct;
                actor.DamageTaken += Actor_DamageTaken;
                actor.Person.Survival.Dead += Survival_Dead;
            }

            while (!humanActor.Person.Survival.IsDead)
            {
                try
                {
                    var humanPersonHp = humanActor.Person
                        .Survival
                        .Stats
                        .Single(x => x.Type == SurvivalStatType.Health)
                        .Value;

                    var hexNode = (HexNode)humanActor.Node;

                    Console.WriteLine($"Current HP: {humanPersonHp} Node {humanActor.Node}");

                    gameLoop.Update();
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"[.] {exception.Message}");
                }
            };

            var scoreFilePreffix = GetProgramArgument(args, SCORE_PREFFIX_ARG);
            WriteScores(tacticContainer, scoreManager, scoreFilePreffix);

            if (!HasProgramArgument(args, SERVER_RUN_ARG))
            {
                Console.ReadLine();
            }
        }

        private static void LoadBotAssembly(string botDirectory, string assemblyName,
            IServiceRegistry serviceRegistry, IServiceFactory serviceFactory)
        {
            var directory = Thread.GetDomain().BaseDirectory;
            var dllPath = Path.Combine(directory, "bots", botDirectory, assemblyName);
            var botAssembly = Assembly.LoadFrom(dllPath);

            // Ищем класс для инициализации.
            var registerManagers = GetTypesWithHelpAttribute<BotRegistrationAttribute>(botAssembly);
            var registerManager = registerManagers.SingleOrDefault();

            // Регистрируем сервис источника команд.
            var botActorTaskSource = GetBotActorTaskSource(registerManager);
            serviceRegistry.Register(typeof(IActorTaskSource), botActorTaskSource, "bot", new PerContainerLifetime());

            var registerAuxMethod = GetMethodByAttribute<RegisterAuxServicesAttribute>(registerManager);
            registerAuxMethod.Invoke(null, new object[] { serviceRegistry });

            var configAuxMethod = GetMethodByAttribute<ConfigureAuxServicesAttribute>(registerManager);
            configAuxMethod.Invoke(null, new object[] { serviceFactory });
        }

        private static IEnumerable<Type> GetTypesWithHelpAttribute<TAttribute>(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(TAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        private static Type GetBotActorTaskSource(Type registerManagerType)
        {
            var props = registerManagerType.GetProperties();
            foreach (var prop in props)
            {
                var actorTaskSourceAttribute = prop.GetCustomAttribute<ActorTaskSourceTypeAttribute>();
                if (actorTaskSourceAttribute != null)
                {
                    return prop.GetValue(null) as Type;
                }
            }

            return null;
        }

        private static MethodInfo GetMethodByAttribute<TAttribute>(Type registerManagerType) where TAttribute: Attribute
        {
            var methods = registerManagerType.GetMethods();
            foreach (var method in methods)
            {
                var specificAttr = method.GetCustomAttribute<TAttribute>();
                if (specificAttr != null)
                {
                    return method;
                }
            }

            return null;
        }


        private static bool HasProgramArgument(string[] args, string testArg)
        {
            return args?.Select(x => x?.Trim().ToLowerInvariant()).Contains(testArg.ToLowerInvariant()) == true;
        }

        private static string GetProgramArgument(string[] args, string testArg)
        {
            foreach (var arg in args)
            {
                var components = arg.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (string.Equals(components[0], testArg, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (components.Length >= 2)
                    {
                        return components[1];
                    }
                }
            }

            return null;
        }

        private static void Survival_Dead(object sender, EventArgs e)
        {
            Console.WriteLine($"{sender} dead");
        }

        private static void Actor_Moved(object sender, EventArgs e)
        {
            var actor = sender as IActor;
            Console.WriteLine($"{actor} moved {actor.Node}");
        }

        private static void WriteSector(ISector sector)
        {
            if (sector.Map is HexMap hexMap)
            {
                hexMap.SaveToFile("map");
            }
        }

        private static void WriteScores(IServiceFactory serviceFactory, IScoreManager scoreManager, string scoreFilePreffix)
        {
            Console.WriteLine("YOU (BOT) DIED");

            Console.WriteLine($"SCORES: {scoreManager.BaseScores}");

            Console.WriteLine("=== You survived ===");
            var minutesTotal = scoreManager.Turns * 2;
            var hoursTotal = minutesTotal / 60f;
            var daysTotal = hoursTotal / 24f;
            var days = (int)daysTotal;
            var hours = (int)(hoursTotal - days * 24);

            Console.WriteLine($"{days} days {hours} hours");
            Console.WriteLine($"Turns: {scoreManager.Turns}");

            Console.WriteLine("=== You visited ===");

            Console.WriteLine($"{scoreManager.Places.Count} places");

            foreach (var placeType in scoreManager.PlaceTypes)
            {
                Console.WriteLine($"{placeType.Key.Name?.En ?? placeType.Key.Name?.Ru ?? placeType.Key.ToString()}: {placeType.Value} turns");
            }

            Console.WriteLine("=== You killed ===");
            foreach (var frag in scoreManager.Frags)
            {
                Console.WriteLine($"{frag.Key.Name?.En ?? frag.Key.Name?.Ru ?? frag.Key.ToString()}: {frag.Value}");
            }

            AppendScores(scoreManager, serviceFactory, scoreFilePreffix);
        }

        private static void AppendScores(IScoreManager scoreManager, IServiceFactory serviceFactory, string scoreFilePreffix)
        {
            var path = SCORE_FILE_PATH;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var botTaskSource = serviceFactory.GetInstance<IActorTaskSource>("bot");
            var scoreFilePreffixFileName = GetScoreFilePreffix(scoreFilePreffix);
            var filename = Path.Combine(path, $"{botTaskSource.GetType().FullName}{scoreFilePreffixFileName}.scores");
            using (var file = new StreamWriter(filename, append: true))
            {
                var fragSum = scoreManager.Frags.Sum(x => x.Value);
                file.WriteLine($"{DateTime.UtcNow}\t{scoreManager.BaseScores}\t{scoreManager.Turns}\t{fragSum}");
            }
        }

        private static string GetScoreFilePreffix(string scoreFilePreffix)
        {
            var scoreFilePreffixFileName = string.Empty;
            if (!string.IsNullOrWhiteSpace(scoreFilePreffix))
            {
                scoreFilePreffixFileName = $"-{scoreFilePreffix}";
            }

            return scoreFilePreffixFileName;
        }

        private static void Actor_DamageTaken(object sender, DamageTakenEventArgs e)
        {
            Console.WriteLine($"{sender} taken {e.Value} damage");
        }

        private static void Actor_UsedAct(object sender, UsedActEventArgs e)
        {
            Console.WriteLine($"{sender} Used act: {e.TacticalAct} Target: {e.Target}");
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
