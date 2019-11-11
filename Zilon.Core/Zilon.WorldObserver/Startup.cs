using LightInject;

using Zilon.Emulation.Common;

namespace Zilon.WorldObserver
{
    public class Startup : InitialzationBase
    {
        public Startup(int diceSeed) : base(diceSeed)
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
