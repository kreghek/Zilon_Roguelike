using Zilon.Bot.Players.Strategies;

namespace Zilon.Bot.Players.NetCore
{
    [BotRegistration]
    public static class RegisterManager
    {
        [ActorTaskSourceType]
        public static Type ActorTaskSourceType => typeof(HumanBotActorTaskSource<ISectorTaskSourceContext>);

        [ConfigureAuxServices]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter",
            Justification = "Относится к параметру serviceFactory, потому что он используется через рефлексию.")]
        public static void ConfigureAuxServices(IServiceProvider serviceFactory)
        {
        }

        [RegisterAuxServices]
        public static void RegisterBot(IServiceCollection serviceRegistry)
        {
            serviceRegistry.RegisterLogicState();
            serviceRegistry.AddScoped<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            serviceRegistry.AddScoped<LogicStateTreePatterns>();
        }
    }
}