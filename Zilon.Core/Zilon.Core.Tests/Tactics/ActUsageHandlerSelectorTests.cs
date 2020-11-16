using System;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Graphs;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ActUsageHandlerSelectorTests
    {
        /// <summary>
        /// Тест проверяет, что селектор возвращает null (НЕ во),
        /// если тип целевого объекта не приводится к целевому типу обработчика.
        /// </summary>
        [Test]
        public void GetHandler_IsNotHandleTarget_ThrowsExceptionHandlerNotFound()
        {
            // ARRANGE

            var testHandlerMock = new Mock<IActUsageHandler>();
            testHandlerMock.SetupGet(x => x.TargetType).Returns(typeof(ITest1));
            var testHandler = testHandlerMock.Object;

            var selector = new ActUsageHandlerSelector(new IActUsageHandler[]
            {
                testHandler
            });

            var targetObject = new NotHandleTarget();

            // ACT
            Action act = () =>
            {
                selector.GetHandler(targetObject);
            };

            // ASSERT

            act.Should().Throw<HandlerNotFoundException>();
        }

        /// <summary>
        /// Тест проверяет, что селектор корректно возвращает обработчик,
        /// если целевой объект напрямую реализует тестовый интерфейс.
        /// </summary>
        [Test]
        public void GetHandler_TargetDirectlyImplementTargetInterfact_ReturnsHandler()
        {
            // ARRANGE

            var testHandlerMock = new Mock<IActUsageHandler>();
            testHandlerMock.SetupGet(x => x.TargetType).Returns(typeof(ITest1));
            var testHandler = testHandlerMock.Object;

            var selector = new ActUsageHandlerSelector(new IActUsageHandler[]
            {
                testHandler
            });

            var targetObject = new Test1();

            // ACT

            var factHandler = selector.GetHandler(targetObject);

            // ASSERT

            factHandler.Should().Be(testHandler);
        }

        /// <summary>
        /// Тест проверяет, что селектор корректно возвращает обработчик,
        /// если целевой объект унаследован от базового класса, который реализует целевой интерфейс.
        /// </summary>
        [Test]
        public void GetHandler_TargetInheritedByBase_ReturnsHandler()
        {
            // ARRANGE

            var testHandlerMock = new Mock<IActUsageHandler>();
            testHandlerMock.SetupGet(x => x.TargetType).Returns(typeof(ITest1));
            var testHandler = testHandlerMock.Object;

            var selector = new ActUsageHandlerSelector(new IActUsageHandler[]
            {
                testHandler
            });

            var targetObject = new Test2();

            // ACT

            var factHandler = selector.GetHandler(targetObject);

            // ASSERT

            factHandler.Should().Be(testHandler);
        }

        private class Test1 : ITest1, IAttackTarget
        {
            public IGraphNode Node { get; }

            public PhysicalSizePattern PhysicalSize { get; }

            public bool CanBeDamaged()
            {
                throw new NotImplementedException();
            }

            public void TakeDamage(int value)
            {
                throw new NotImplementedException();
            }
        }

        private class Test2 : Test1
        {
        }

        private sealed class NotHandleTarget : IAttackTarget
        {
            public IGraphNode Node { get; }

            public PhysicalSizePattern PhysicalSize { get; }

            public bool CanBeDamaged()
            {
                throw new NotImplementedException();
            }

            public void TakeDamage(int value)
            {
                throw new NotImplementedException();
            }
        }

        private interface ITest1
        {
        }
    }
}