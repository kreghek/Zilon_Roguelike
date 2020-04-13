using NUnit.Framework;

using Zilon.Core.Tactics;
using Zilon.Core.Tests.Tactics.Base;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class PropContainerManagerTests : CommonManagerTestsBase<IPropContainer>
    {
        protected override ISectorEntityManager<IPropContainer> CreateManager()
        {
            var propContainerManager = new StaticObjectManager();
            return propContainerManager;
        }
    }
}