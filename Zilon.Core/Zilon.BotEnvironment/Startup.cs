using LightInject;

using Zilon.Emulation.Common;

namespace Zilon.Bot
{
    class Startup: InitialzationBase
    {
        public Startup(string catalogPath) : base(catalogPath)
        {
        }

        public override void ConfigureAux(IServiceFactory serviceFactory)
        {
            
        }

        protected override void RegisterBot(IServiceRegistry container)
        {
            
        }
    }
}
