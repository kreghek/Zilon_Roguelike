using Moq;
using NUnit.Framework;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Generation;

namespace Zilon.CoreTests.Tactics.Generation
{
    using System;

    using FluentAssertions;

    using Zilon.Core.Players;
    using Zilon.Core.Tactics.Spatial;

    [TestFixture()]
    public class SectorProceduralGeneratorTests
    {
        [Test()]
        public void GenerateTest()
        {
            // ARRANGE
            var randomSourceMock = new Mock<ISectorGeneratorRandomSource>();
            var randomSource = randomSourceMock.Object;

            var sectorMock = new Mock<ISector>();
            var sector = sectorMock.Object;

            var mapMock = new Mock<IMap>();
            var map = mapMock.Object;

            var playerActorMock = new Mock<IActor>();
            var actor = playerActorMock.Object;


            var generator = new SectorProceduralGenerator(randomSource);


            // ACT
            Action act = ()=>
            {
                generator.Generate(sector, map, actor);
            };


            // ASSERT
            act.Should().NotThrow();
        }
    }
}