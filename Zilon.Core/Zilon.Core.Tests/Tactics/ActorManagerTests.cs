using NUnit.Framework;

using Zilon.Core.Tactics;
using Zilon.Core.Tests.Tactics.Base;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class ActorManagerTests : CommonManagerTestsBase<IActor>
    {
        protected override ISectorEntityManager<IActor> CreateManager()
        {
            var actorManager = new ActorManager();
            return actorManager;
        }
    }
}