using System.Diagnostics.CodeAnalysis;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tests.StaticObjectModules.TestCaseDataSources;

namespace Zilon.Core.Tests.StaticObjectModules
{
    [TestFixture]
    public class DepositDurabilityModuleTests
    {
        [TestCaseSource(typeof(DepositDurabilityModuleTestCaseDataSource),
            nameof(DepositDurabilityModuleTestCaseDataSource.DestroyTestCases))]
        [Test]
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "<Pending>")]
        public void TakeDamageTest(int stockValue, int damagePerMineUnit, int[] damages)
        {
            int currentStock = stockValue;
            var depositModuleMock = new Mock<IPropDepositModule>();
            depositModuleMock.SetupGet(x => x.Stock).Returns(() => (float)currentStock / stockValue);
            depositModuleMock.Setup(x => x.Mine()).Callback(() => currentStock--);
            var depositModule = depositModuleMock.Object;

            var lifetimeModuleMock = new Mock<ILifetimeModule>();
            var lifetimeModule = lifetimeModuleMock.Object;

            DepositDurabilityModule durabilityModule =
                new DepositDurabilityModule(depositModule, lifetimeModule, damagePerMineUnit);

            // ACT
            foreach (int damage in damages)
            {
                durabilityModule.TakeDamage(damage);
            }

            // ASSERT
            lifetimeModuleMock.Verify(x => x.Destroy(), Times.Once);
        }
    }
}