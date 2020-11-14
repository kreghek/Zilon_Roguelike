using System;
using Microsoft.Extensions.DependencyInjection;
using Zilon.Emulation.Common;

namespace Zilon.Core.Benchmarks.Move
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
            // Регистрация в Program.
        }
    }
}