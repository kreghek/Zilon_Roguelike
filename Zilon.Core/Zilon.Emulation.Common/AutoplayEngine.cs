using System;

using Zilon.Bot.Sdk;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Emulation.Common
{
    public class AutoplayEngine : AutoplayEngineBase
    {
        private readonly InitializationBase _startup;

        public AutoplayEngine(InitializationBase startup, BotSettings botSettings,
            IGlobeInitializer globeInitializer) : base(
            botSettings, globeInitializer)
        {
            _startup = startup;
        }

        protected override void CatchActorTaskExecutionException(ActorTaskExecutionException exception)
        {
            Console.WriteLine(exception);
            throw exception;
        }

        protected override void ConfigBotAux()
        {
            _startup.ConfigureAux(ServiceScope.ServiceProvider);
        }

        protected override void ProcessEnd()
        {
            // В тестх не требуется пост-обратоки результатов.
            // Здесь должна быть фиксация итогов - очки, причина смерти, состояние персонажа на момент смерти.
        }

        protected override void ProcessSectorExit()
        {
            // Do nothing in bench
        }
    }
}