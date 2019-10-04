using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LightInject;

using Zilon.Bot;
using Zilon.Bot.Sdk;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.BotEnvironment
{
    class Program
    {
        private const string SERVER_RUN_ARG = "ServerRun";
        private const string SCORE_PREFFIX_ARG = "ScorePreffix";
        private const string BOT_MODE_ARG = "Mode";
        private const string SCORE_FILE_PATH = "bot-scores";
        private const int ITERATION_LIMIT = 40_000;
        private const int BOT_EXCEPTION_LIMIT = 3;
        private const int ENVIRONMENT_EXCEPTION_LIMIT = 3;

        private static ServiceContainer _globalServiceContainer;
        private static Startup _startUp;
        private static Scope _sectorServiceContainer;
        private static bool _changeSector;

        static async Task Main(string[] args)
        {
            var scoreFilePreffix = GetProgramArgument(args, SCORE_PREFFIX_ARG);

            _globalServiceContainer = new ServiceContainer();
            _startUp = new Startup();
            _startUp.RegisterServices(_globalServiceContainer);

            LoadBotAssembly("cdt", "Zilon.Bot.Players.LightInject.dll", _globalServiceContainer, _globalServiceContainer);

            var humanActor = await CreateSectorAsync();

            var gameLoop = _sectorServiceContainer.GetInstance<IGameLoop>();
            var botActorTaskSource = _sectorServiceContainer.GetInstance<ISectorActorTaskSource>("bot");
            ConfigureBot(args, botActorTaskSource);

            var botExceptionCount = 0;
            var envExceptionCount = 0;
            var iterationCounter = 1;
            while (!humanActor.Person.Survival.IsDead && iterationCounter <= ITERATION_LIMIT)
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
                catch (ActorTaskExecutionException exception)
                {
                    AppendException(exception, scoreFilePreffix);

                    var monsterActorTaskSource = _sectorServiceContainer.GetInstance<IActorTaskSource>("monster");
                    if (exception.ActorTaskSource != monsterActorTaskSource)
                    {
                        botExceptionCount++;

                        if (botExceptionCount >= BOT_EXCEPTION_LIMIT)
                        {
                            AppendFail(_globalServiceContainer, scoreFilePreffix);
                            throw;
                        }
                    }
                    else
                    {
                        envExceptionCount++;
                        CheckEnvExceptions(envExceptionCount, exception);
                        Console.WriteLine($"[.] {exception.Message}");
                    }
                }
                catch (Exception exception)
                {
                    AppendException(exception, scoreFilePreffix);

                    envExceptionCount++;
                    CheckEnvExceptions(envExceptionCount, exception);
                    Console.WriteLine($"[.] {exception.Message}");
                }

                if (_changeSector)
                {
                    humanActor = await CreateSectorAsync();

                    gameLoop = _sectorServiceContainer.GetInstance<IGameLoop>();

                    _changeSector = false;
                }

                iterationCounter++;
            };

            var mode = GetProgramArgument(args, BOT_MODE_ARG);
            var scoreManager = _globalServiceContainer.GetInstance<IScoreManager>();
            WriteScores(_globalServiceContainer, scoreManager, mode, scoreFilePreffix);

            if (!HasProgramArgument(args, SERVER_RUN_ARG))
            {
                Console.ReadLine();
            }
        }

        private static void ConfigureBot(string[] args, ISectorActorTaskSource actorTaskSource)
        {
            var botSettings = new BotSettings
            {
                Mode = GetProgramArgument(args, BOT_MODE_ARG)
            };
            actorTaskSource.Configure(botSettings);
        }

        private static void CheckEnvExceptions(int envExceptionCount, Exception exception)
        {
            if (envExceptionCount >= ENVIRONMENT_EXCEPTION_LIMIT)
            {
                throw exception;
            }
        }

        private static async Task<IActor> CreateSectorAsync()
        {
            if (_sectorServiceContainer != null && !_sectorServiceContainer.IsDisposed)
            {
                DropActorEventSubscriptions();

                _sectorServiceContainer.Dispose();
            }

            _sectorServiceContainer = _globalServiceContainer.BeginScope();

            _startUp.ConfigureAux(_sectorServiceContainer);

            var schemeService = _globalServiceContainer.GetInstance<ISchemeService>();
            var humanPlayer = _globalServiceContainer.GetInstance<HumanPlayer>();
            var survivalRandomSource = _globalServiceContainer.GetInstance<ISurvivalRandomSource>();
            var personFactory = _globalServiceContainer.GetInstance<IHumanPersonFactory>();
            var propFactory = _globalServiceContainer.GetInstance<IPropFactory>();
            var scoreManager = _globalServiceContainer.GetInstance<IScoreManager>();
            var perkResolver = _globalServiceContainer.GetInstance<IPerkResolver>();

            var gameLoop = _sectorServiceContainer.GetInstance<IGameLoop>();
            var sectorManager = _sectorServiceContainer.GetInstance<ISectorManager>();
            var botActorTaskSource = _sectorServiceContainer.GetInstance<ISectorActorTaskSource>("bot");
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
                personFactory,
                sectorManager,
                actorManager,
                perkResolver);

            CreateActorEventSubscriptions();

            return humanActor;
        }

        private static void CreateActorEventSubscriptions()
        {
            var actorManager = _sectorServiceContainer.GetInstance<IActorManager>();
            foreach (var actor in actorManager.Items)
            {
                actor.Moved += Actor_Moved;
                actor.UsedAct += Actor_UsedAct;
                actor.DamageTaken += Actor_DamageTaken;
                actor.Person.Survival.Dead += Survival_Dead;
            }
        }

        private static void DropActorEventSubscriptions()
        {
            var actorManager = _sectorServiceContainer.GetInstance<IActorManager>();
            foreach (var actor in actorManager.Items)
            {
                actor.Moved -= Actor_Moved;
                actor.UsedAct -= Actor_UsedAct;
                actor.DamageTaken -= Actor_DamageTaken;
                actor.Person.Survival.Dead -= Survival_Dead;
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
            serviceRegistry.Register(typeof(ISectorActorTaskSource), botActorTaskSource, "bot", new PerScopeLifetime());
            serviceRegistry.Register<IActorTaskSource>(factory => factory.GetInstance<ISectorActorTaskSource>(), "bot",
                new PerScopeLifetime());

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

        private static MethodInfo GetMethodByAttribute<TAttribute>(Type registerManagerType) where TAttribute : Attribute
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

        private static void WriteScores(IServiceFactory serviceFactory, IScoreManager scoreManager, string mode, string scoreFilePreffix)
        {
            var summaryText = TextSummaryHelper.CreateTextSummary(scoreManager.Scores);

            Console.WriteLine(summaryText);

            AppendScores(scoreManager, serviceFactory, scoreFilePreffix, mode, summaryText);
        }

        private static void AppendScores(IScoreManager scoreManager, IServiceFactory serviceFactory, string scoreFilePreffix, string mode, string summary)
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

            DatabaseContext.AppendScores(scoreManager, serviceFactory, scoreFilePreffix, mode, summary);
        }

        private static void AppendFail(IServiceFactory serviceFactory, string scoreFilePreffix)
        {
            Console.WriteLine("[x] Bot task source error limit reached");

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
                file.WriteLine($"-1");
            }
        }

        private static void AppendException(Exception exception, string scoreFilePreffix)
        {
            var path = SCORE_FILE_PATH;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var botTaskSource = _sectorServiceContainer.GetInstance<IActorTaskSource>("bot");
            var scoreFilePreffixFileName = GetScoreFilePreffix(scoreFilePreffix);
            var filename = Path.Combine(path, $"{botTaskSource.GetType().FullName}{scoreFilePreffixFileName}.exceptions");
            using (var file = new StreamWriter(filename, append: true))
            {
                file.WriteLine(DateTime.UtcNow);
                file.WriteLine(exception);
                file.WriteLine();
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

        private static void CurrentSector_HumanGroupExit(object sender, SectorExitEventArgs e)
        {
            Console.WriteLine("Exit");

            _changeSector = true;
            var sectorManager = _sectorServiceContainer.GetInstance<ISectorManager>();
            sectorManager.CurrentSector.HumanGroupExit -= CurrentSector_HumanGroupExit;
        }

        private static IActor CreateHumanActor(HumanPlayer humanPlayer,
            IHumanPersonFactory personFactory,
            ISectorManager sectorManager,
            IActorManager actorManager,
            IPerkResolver perkResolver)
        {
            var person = personFactory.Create();

            Console.WriteLine("Start properties:");
            foreach (var equipment in person.EquipmentCarrier)
            {
                Console.WriteLine($"Equiped: {equipment}");
            }

            foreach (var prop in person.Inventory.CalcActualItems())
            {
                Console.WriteLine(prop);
            }

            humanPlayer.MainPerson = person;

            var playerActorStartNode = sectorManager.CurrentSector.Map.Regions
                .SingleOrDefault(x => x.IsStart).Nodes
                .First();

            var actor = new Actor(humanPlayer.MainPerson, humanPlayer, playerActorStartNode, perkResolver);

            actorManager.Add(actor);

            return actor;
        }
    }
}
