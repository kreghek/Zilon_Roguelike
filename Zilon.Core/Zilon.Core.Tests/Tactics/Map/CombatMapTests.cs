namespace Zilon.Logic.Tests.Tactics.Map
{
    using FluentAssertions;

    using NUnit.Framework;

    using Zilon.Logic.Tactics.Map;

    [TestFixture]
    public class CombatMapTests
    {
        /// <summary>
        /// 1. В системе есть дефолный набор нод.
        /// 2. Создаём карту.
        /// 3. Карта успешно создана. Ноды указаны в объекте карты.
        /// </summary>
        [Test]
        public void Constructor_Default_HasNodes()
        {
            //TODO Карта должна создаваться на основе указанного набора нодов, передаваемых в конструкторе.
            var map = new CombatMap();

            map.Should().NotBeNull();
            map.Nodes.Should().NotBeNull();
            map.TeamNodes.Should().NotBeNull();
        }
    }
}