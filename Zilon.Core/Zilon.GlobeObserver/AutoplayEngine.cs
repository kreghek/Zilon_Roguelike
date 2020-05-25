using System;

using Zilon.Bot.Sdk;
using Zilon.Core.Tactics;
using Zilon.Emulation.Common;

namespace Zilon.GlobeObserver
{
    class AutoplayEngine<T> : AutoplayEngineBase<T> where T : IPluggableActorTaskSource
    {
        private readonly StartUp _startup;

        public AutoplayEngine(StartUp startup, BotSettings botSettings) : base(botSettings)
        {
            _startup = startup;
        }

        protected override void ConfigBotAux()
        {
            _startup.ConfigureAux(ServiceScope.ServiceProvider);
        }

        protected override void CatchActorTaskExecutionException(ActorTaskExecutionException exception)
        {
            Console.WriteLine(exception);
            throw exception;
        }

        protected override void CatchException(Exception exception)
        {
            Console.WriteLine(exception);
        }

        protected override void ProcessEnd()
        {
            // В тестх не требуется пост-обратоки результатов.
            // Здесь должна быть фиксация итогов - очки, причина смерти, состояние персонажа на момент смерти.
        }

        protected override void ProcessSectorExit()
        {
            Console.WriteLine("Exit");
        }
    }
}
