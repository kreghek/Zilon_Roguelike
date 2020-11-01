using Zilon.Core.Tactics;
using Zilon.Core.Tests.Tactics.Base;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class StaticObjectManagerTests : CommonManagerTestsBase<IStaticObject>
    {
        /// <inheritdoc />
        protected override ISectorEntityManager<IStaticObject> CreateManager()
        {
            StaticObjectManager staticObjectManager = new StaticObjectManager();
            return staticObjectManager;
        }
    }
}