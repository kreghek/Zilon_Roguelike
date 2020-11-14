using Zilon.Core.Tactics;
using Zilon.Core.Tests.Tactics.Base;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ActorManagerTests : CommonManagerTestsBase<IActor>
    {
        protected override ISectorEntityManager<IActor> CreateManager()
        {
            ActorManager actorManager = new ActorManager();
            return actorManager;
        }
    }
}