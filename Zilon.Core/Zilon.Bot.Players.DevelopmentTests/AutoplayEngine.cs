﻿using System;

using NUnit.Framework;

using Zilon.Bot.Sdk;
using Zilon.Core.Tactics;
using Zilon.Core.World;
using Zilon.Emulation.Common;

namespace Zilon.Bot.Players.DevelopmentTests
{
    internal class AutoplayEngine : AutoplayEngineBase
    {
        private readonly Startup _startup;

        public AutoplayEngine(Startup startup, BotSettings botSettings, IGlobeInitializer globeInitializer) : base(
            botSettings, globeInitializer)
        {
            _startup = startup;
        }

        protected override void CatchActorTaskExecutionException(ActorTaskExecutionException exception)
        {
            TestContext.Out.WriteLine(exception);
            throw exception;
        }

        protected override void CatchException(Exception exception)
        {
            TestContext.Out.WriteLine(exception);
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
            TestContext.Out.WriteLine("Exit");
        }
    }
}