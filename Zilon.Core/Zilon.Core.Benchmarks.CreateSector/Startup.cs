﻿using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Emulation.Common;

namespace Zilon.Core.Benchmarks.CreateSector
{
    internal class Startup : InitializationBase
    {
        public Startup() : base(123)
        {
        }

        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
            // Сейчас конфигурирование доп.сервисов для базового бота не требуется.
        }

        protected override void RegisterBot(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterBot();
        }
    }
}