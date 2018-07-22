using NUnit.Framework;
using Zilon.Core.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.CommonServices.Dices;
using Moq;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class DropResolverTests
    {
        [Test()]
        public void GetPropsTest()
        {
            // ARRANGE
            var diceMock = new Mock<IDice>();
            var dice = diceMock.Object;

            var resolver = new DropResolver(dice)
        }
    }
}