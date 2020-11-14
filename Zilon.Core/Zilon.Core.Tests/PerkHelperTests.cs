using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class PerkHelperTests
    {
        private TestPerkScheme _perkScheme;

        [Test]
        public void ConvertTotalLevel_TotalLevelFor2_Returns1Level1Sublevel()
        {
            //ARRANGE
            const int testedTotalLevel = 2;
            const int expectedLevel = 1;
            const int expectedSubLevel = 2;




            // ACT
            PerkHelper.ConvertTotalLevel(_perkScheme, testedTotalLevel,
                out int? factLevel,
                out int? factSubLevel);



            // ASSERT
            factLevel.Should().Be(expectedLevel);
            factSubLevel.Should().Be(expectedSubLevel);
        }

        [Test]
        public void ConvertTotalLevel_TotalLevelFor6_Returns2Level1Sublevel()
        {
            //ARRANGE
            const int testedTotalLevel = 6;
            const int expectedLevel = 2;
            const int expectedSubLevel = 1;




            // ACT
            PerkHelper.ConvertTotalLevel(_perkScheme, testedTotalLevel,
                out int? factLevel,
                out int? factSubLevel);



            // ASSERT
            factLevel.Should().Be(expectedLevel);
            factSubLevel.Should().Be(expectedSubLevel);
        }

        [SetUp]
        public void SetUp()
        {
            _perkScheme = new TestPerkScheme
            {
                Levels = new[] {
                    new PerkLevelSubScheme()
                    {
                        MaxValue = 5
                    },
                     new PerkLevelSubScheme()
                     {
                        MaxValue = 2
                    },
                      new PerkLevelSubScheme()
                      {
                        MaxValue = 3
                    }
                }
            };
        }
    }
}