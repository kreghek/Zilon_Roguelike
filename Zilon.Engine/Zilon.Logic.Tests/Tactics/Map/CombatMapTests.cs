using NUnit.Framework;
using Zilon.Logic.Tactics.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Zilon.Logic.Tactics.Map.Tests
{
    [TestFixture()]
    public class CombatMapTests
    {
        [Test()]
        public void CombatMapTest()
        {
            var map = new CombatMap();

            map.Should().NotBeNull();
            map.Nodes.Should().NotBeNull();
            map.TeamNodes.Should().NotBeNull();
        }
    }
}