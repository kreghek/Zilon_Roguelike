using Moq;

using NUnit.Framework;

using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules.Tests
{
    [TestFixture]
    public sealed class MonsterMovingModuleTests
    {
        [Test]
        [TestCaseSource(typeof(MonsterMovingModuleTestCaseSource),
            nameof(MonsterMovingModuleTestCaseSource.PositiveTestCases))]
        public int CalculateCost_PositivesFromTestCases_ExpectedValues(float moveFactor)
        {
            // ARRANGE

            var scheme = Mock.Of<IMonsterScheme>(x => x.MoveSpeedFactor == moveFactor);

            var module = new MonsterMovingModule(scheme);

            // ACT

            var factMoveCost = module.CalculateCost();

            // ASSERT

            return factMoveCost;
        }
    }
}