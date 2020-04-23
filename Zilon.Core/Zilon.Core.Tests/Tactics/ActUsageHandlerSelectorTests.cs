using NUnit.Framework;
using Zilon.Core.Tactics;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using FluentAssertions;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture()]
    public class ActUsageHandlerSelectorTests
    {
        [Test()]
        public void GetHandlerTest()
        {
            // ARRANGE

            var testHandlerMock = new Mock<IActUsageHandler>();

            var testHandler = testHandlerMock.Object;

            var selector = new ActUsageHandlerSelector(new IActUsageHandler[] { testHandler });

            var targetObject = new Test1();

            // ACT

            var factHandler = selector.GetHandler(targetObject);

            // ASSERT

            factHandler.Should().Be(testHandler);
        }

        private sealed class Test1 : IAttackTarget
        {
            public IGraphNode Node { get; }
            public PhysicalSize PhysicalSize { get; }

            public bool CanBeDamaged()
            {
                throw new NotImplementedException();
            }

            public void TakeDamage(int value)
            {
                throw new NotImplementedException();
            }
        }
    }
}