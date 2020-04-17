using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Zilon.Core.Graphs;
using Moq;
using Zilon.Core.StaticObjectModules;
using FluentAssertions;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics.Tests
{
    [TestFixture]
    public class StaticObjectTests
    {
        [Test]
        public void AddModule_TestModuleWithAdditionalInterfact_ReturnsTestModule()
        {
            // ARRANGE
            var nodeMock = new Mock<IGraphNode>();
            var node = nodeMock.Object;

            var staticObject = new StaticObject(node, default);

            var testModule = new TestModule();

            // ACT

            staticObject.AddModule(testModule);
            var factTestModule = staticObject.GetModule<ITestModule>();

            // ASSERT
            factTestModule.Should().NotBeNull();
            factTestModule.Should().BeOfType<TestModule>();
        }

        [Test]
        public void AddModule_From()
        {
            // ARRANGE

            var dropResolverMock = new Mock<IDropResolver>();
            var dropResolver = dropResolverMock.Object;

            var container = new DropTablePropChest(Array.Empty<IDropTableScheme>(), dropResolver);

            var nodeMock = new Mock<IGraphNode>();
            var node = nodeMock.Object;

            var staticObject = new StaticObject(node, default);

            // ACT
            staticObject.AddModule(container);
            var factTestModule = staticObject.GetModule<IPropContainer>();

            // ASSERT
            factTestModule.Should().BeOfType<DropTablePropChest>();
        }

        [StaticObjectModule]
        private interface ITestModule: IStaticObjectModule
        {
        }

        private sealed class TestModule : IAdditionalInterface, ITestModule
        {
            public bool IsActive { get; set; }
            public string Key { get => nameof(ITestModule); }
        }

        private interface IAdditionalInterface
        { 
        }
    }
}