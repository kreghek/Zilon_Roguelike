using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Emulation.Common;

namespace Zilon.BotEnvironment
{
    class Startup : InitializationBase
    {
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