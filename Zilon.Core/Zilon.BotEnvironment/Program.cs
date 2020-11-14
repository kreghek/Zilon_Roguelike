using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Zilon.CommonUtilities;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.BotEnvironment
{
    internal class Program
    {
        private const string SERVER_RUN_ARG = "ServerRun";
        private const string SCORE_PREFFIX_ARG = "ScorePreffix";
        private const string BOT_MODE_ARG = "Mode";
        private static Startup _startUp;

        private static async Task Main(string[] args)
        {
            string scoreFilePreffix = ArgumentHelper.GetProgramArgument(args, SCORE_PREFFIX_ARG);

            var serviceCollection = new ServiceCollection();

            _startUp = new Startup();
            _startUp.RegisterServices(serviceCollection);

            var botSettings = new BotSettings {Mode = ArgumentHelper.GetProgramArgument(args, BOT_MODE_ARG)};

            var serviceProvider = serviceCollection.BuildServiceProvider();
            LoadBotAssembly("cdt", "Zilon.Bot.Players.NetCore.dll", serviceCollection, serviceProvider);
            var serviceProviderWithDynamicBotServices = serviceCollection.BuildServiceProvider();

            var globeInitializer = serviceProviderWithDynamicBotServices.GetRequiredService<IGlobeInitializer>();

            AutoplayEngine autoPlayEngine =
                new AutoplayEngine(_startUp, botSettings, scoreFilePreffix, globeInitializer);

            var player = serviceProvider.GetRequiredService<IPlayer>();
            var startPerson = player.MainPerson;

            var globe = await autoPlayEngine.CreateGlobeAsync();

            await autoPlayEngine.StartAsync(globe, startPerson);

            Console.WriteLine(autoPlayEngine.LogOutput);

            if (!ArgumentHelper.HasProgramArgument(args, SERVER_RUN_ARG))
            {
                Console.ReadLine();
            }
        }

        private static void LoadBotAssembly(string botDirectory, string assemblyName,
            IServiceCollection serviceRegistry, IServiceProvider serviceFactory)
        {
            string? directory = Thread.GetDomain().BaseDirectory;
            string dllPath = Path.Combine(directory, "bots", botDirectory, assemblyName);
            Assembly botAssembly = Assembly.LoadFrom(dllPath);

            // Ищем класс для инициализации.
            IEnumerable<Type> registerManagers = GetTypesWithHelpAttribute<BotRegistrationAttribute>(botAssembly);
            Type registerManager = registerManagers.SingleOrDefault();

            // Регистрируем сервис источника команд.
            Type botActorTaskSourceType = GetBotActorTaskSource(registerManager);
            serviceRegistry.AddScoped(typeof(IPluggableActorTaskSource<ISectorTaskSourceContext>),
                botActorTaskSourceType);
            serviceRegistry.AddScoped<IActorTaskSource<ISectorTaskSourceContext>>(factory =>
                factory.GetRequiredService<IPluggableActorTaskSource<ISectorTaskSourceContext>>());

            MethodInfo registerAuxMethod = GetMethodByAttribute<RegisterAuxServicesAttribute>(registerManager);
            registerAuxMethod.Invoke(null, new object[] {serviceRegistry});

            MethodInfo configAuxMethod = GetMethodByAttribute<ConfigureAuxServicesAttribute>(registerManager);
            configAuxMethod.Invoke(null, new object[] {serviceFactory});
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
            PropertyInfo[] props = registerManagerType.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                var actorTaskSourceAttribute = prop.GetCustomAttribute<ActorTaskSourceTypeAttribute>();
                if (actorTaskSourceAttribute != null)
                {
                    return prop.GetValue(null) as Type;
                }
            }

            return null;
        }

        private static MethodInfo GetMethodByAttribute<TAttribute>(Type registerManagerType)
            where TAttribute : Attribute
        {
            MethodInfo[] methods = registerManagerType.GetMethods();
            foreach (MethodInfo method in methods)
            {
                TAttribute? specificAttr = method.GetCustomAttribute<TAttribute>();
                if (specificAttr != null)
                {
                    return method;
                }
            }

            return null;
        }
    }
}