using System;

using FluentAssertions;

using Moq;

using NUnit.Framework;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.TestCommon;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class SectorTests
    {
        /// <summary>
        /// Тест проверяет выполнение обновления состояния сектора.
        /// Есть квадратная карта. В произвольных местах расположены два монстра и адин актёр игрока.
        /// Игрок ничего не делает. Монстры должны выполнять логику патрулирования.
        /// Это длится 100 ходов. Не должно быть выбрасываться NRE.
        /// </summary>
        [Test(Description = "Stc1")]
        [Ignore("Ещё не доделан. Нужно перенести наработки из клиента.")]
        public void Update_3Actor2MonsterPatrols100SectorUpdates_NoNRE()
        {
            // ARRANGE
            var map = new TestGridGenMap();

            var actorManagerMock = new Mock<IActorManager>();
            var actorManager = actorManagerMock.Object;

            var sector = new Sector(map, actorManager);


            // ACT
            Action act = () => { sector.Update(); };



            // ASSERT
            act.Should().NotThrow();
        }

        private void GenerateSectorStc1Content(Sector sector, IActorManager actorManager, IMap map)
        {
            var humanPlayer = new Mock<IPlayer>();

        }
    }
}