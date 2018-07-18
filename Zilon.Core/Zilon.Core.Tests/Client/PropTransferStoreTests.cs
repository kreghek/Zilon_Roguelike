using NUnit.Framework;
using Zilon.Core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using FluentAssertions;

namespace Zilon.Core.Client.Tests
{
    [TestFixture]
    public class PropTransferStoreTests
    {
        /// <summary>
        /// Тест проверяет, что при добавлении в пустой инвентарь 1 единицы ресурса
        /// на выходе будет 1 единица этого ресурса.
        /// </summary>
        [Test]
        public void Add_AddResourceToEmptyInventory_PropContainsThisResource()
        {
            // ARRANGE
            const int expectedCount = 1;

            var realStoreMock = new Mock<IPropStore>();
            var props = new IProp[] { };
            realStoreMock.SetupGet(x => x.Items).Returns(props);
            var realStore = realStoreMock.Object;

            var testedScheme = new PropScheme { };
            var testedResource = new Resource(testedScheme, expectedCount);

            var propTransferStore = new PropTransferStore(realStore);



            // ACT
            propTransferStore.Add(testedResource);


            // ASSERT
            var factProps = propTransferStore.Items;
            factProps[0].Should().BeOfType<Resource>();
            factProps[0].Scheme.Should().Be(testedScheme);
            ((Resource)factProps[0]).Count.Should().Be(expectedCount);
        }
    }
}