using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DropTableChestStoreTests
    {
        /// <summary>
        ///     Тест проверяет, что если дропнулось 2 одинаковых ресурса, то они объединяются в один стек.
        /// </summary>
        [Test]
        public void CalcActualItems_DropSameResourceTwice_MergeSameResourcesToStack()
        {
            // ARRANGE

            PropScheme scheme = new PropScheme();

            var dropResolverMock = new Mock<IDropResolver>();
            dropResolverMock.Setup(x => x.Resolve(It.IsAny<DropTableScheme[]>()))
                .Returns(new IProp[] {CreateFakeResource(scheme), CreateFakeResource(scheme)});
            var dropResolver = dropResolverMock.Object;

            DropTableChestStore store = new DropTableChestStore(new DropTableScheme[0], dropResolver);


            // ACT
            IProp[] props = store.CalcActualItems();


            // ASSERT
            props.Length.Should().Be(1);
            ((Resource)props[0]).Count.Should().Be(2);
        }

        /// <summary>
        ///     Тест проверяет, что контент сундуков разрешается только при первом обращении.
        /// </summary>
        [Test]
        public void CalcActualItems_GetContentItems_ResolveContentOnce()
        {
            // ARRANGE

            PropScheme scheme = new PropScheme();

            var dropResolverMock = new Mock<IDropResolver>();
            dropResolverMock.Setup(x => x.Resolve(It.IsAny<DropTableScheme[]>()))
                .Returns(new IProp[] {CreateFakeResource(scheme)});
            var dropResolver = dropResolverMock.Object;

            DropTableChestStore store = new DropTableChestStore(new DropTableScheme[0], dropResolver);
            IProp[] firstProps = store.CalcActualItems();


            // ACT
            IProp[] secondProps = store.CalcActualItems();


            // ASSERT
            dropResolverMock.Verify(x => x.Resolve(It.IsAny<DropTableScheme[]>()), Times.Once);
            secondProps.Length.Should().Be(firstProps.Length);
            secondProps[0].Should().BeSameAs(firstProps[0]);
        }

        private Resource CreateFakeResource(PropScheme scheme)
        {
            Resource resource = new Resource(scheme, 1);
            return resource;
        }
    }
}