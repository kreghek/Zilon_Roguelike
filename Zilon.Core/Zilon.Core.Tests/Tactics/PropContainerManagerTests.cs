using NUnit.Framework;

using Zilon.Core.Tactics;
using Zilon.Core.Tests.Tactics.Base;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class PropContainerManagerTests : CommonManagerTestsBase<IPropContainer>
    {
        protected override ISectorEntityManager<IPropContainer> CreateManager()
        {
            var propContainerManager = new PropContainerManager();
            return propContainerManager;
        }
    }
}