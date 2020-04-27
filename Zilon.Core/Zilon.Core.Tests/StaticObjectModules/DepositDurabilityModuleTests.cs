using Moq;

using NUnit.Framework;

namespace Zilon.Core.StaticObjectModules.Tests
{
    [TestFixture()]
    public class DepositDurabilityModuleTests
    {
        [Test()]
        public void TakeDamageTest()
        {
            const int StockValue = 1;

            const int DamagePerMineUnit = 10;

            var damages = new int[] { 6, 4 };

            var currentStock = StockValue;
            var depositModuleMock = new Mock<IPropDepositModule>();
            depositModuleMock.SetupGet(x => x.Stock).Returns(() => (float)currentStock / StockValue);
            depositModuleMock.Setup(x => x.Mine()).Callback(() => currentStock--);
            var depositModule = depositModuleMock.Object;

            var lifetimeModuleMock = new Mock<ILifetimeModule>();
            var lifetimeModule = lifetimeModuleMock.Object;
            

            var durabilityModule = new DepositDurabilityModule(depositModule, lifetimeModule, DamagePerMineUnit);

            // ACT
            foreach (var damage in damages)
            {
                durabilityModule.TakeDamage(damage);
            }

            // ASSERT
            lifetimeModuleMock.Verify(x => x.Destroy(), Times.Once);
        }
    }
}