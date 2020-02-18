using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Emulation.Common;

namespace Zilon.BotEnvironment
{
    class Startup : InitialzationBase
    {
        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
        }

        protected override void RegisterBot(IServiceCollection serviceCollection)
        {
        }
    }
}
