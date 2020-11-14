using System;
using Microsoft.Extensions.DependencyInjection;
using Zilon.Emulation.Common;

namespace Zilon.Core.MassSectorGenerator
{
    public class Startup : InitializationBase
    {
        public Startup(int diceSeed) : base(diceSeed)
        {
        }

        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
            // Для этой утилиты бота не настраиваем. Нам нужны только сектора.
        }

        protected override void RegisterBot(IServiceCollection serviceCollection)
        {
            // Для этой утилиты бота не настраиваем. Нам нужны только сектора.
        }
    }
}