using System.Collections.Generic;
using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Props
{
    [TestFixture]
    public class PropStoreBaseTests
    {
        /// <summary>
        /// Тест проверяет, что при добавлении экипировки она помещается в контейнер.
        /// </summary>
        [Test]
        public void Add_Equipment_EquipmentInItems()
        {
            // ARRANGE
            const int expectedItemCount = 1;

            var propScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };
            var equipment = new Equipment(propScheme, new ITacticalActScheme[0]);

            var propStore = CreatePropStore();



            // ACT
            propStore.Add(equipment);



            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().HaveCount(expectedItemCount);
            items[0].Should().Be(equipment);
        }

        /// <summary>
        /// Тест проверяет, что при добавлении ресурса он помещается в контейнер.
        /// </summary>
        [Test]
        public void Add_Resource_ResourceInItems()
        {
            // ARRANGE
            const int expectedItemCount = 1;
            const int resourceInitCount = 1;

            var propScheme = new TestPropScheme
            {
                
            };
            var resource = new Resource(propScheme, resourceInitCount);

            var propStore = CreatePropStore();



            // ACT
            propStore.Add(resource);



            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().HaveCount(expectedItemCount);
            items[0].Should().Be(resource);
        }

        /// <summary>
        /// Тест проверяет, что при добавлении ресурса, который уже есть в инвентаре,
        /// количество ресурса в контейнере увеличивается.
        /// </summary>
        [Test]
        public void Add_ResourceStack_ResourceInItemsCountIncreace()
        {
            // ARRANGE
            const int expectedItemCount = 1;
            const int resourceInitCount1 = 1;
            const int resourceInitCount2 = 3;
            const int expectedResourceCount = resourceInitCount1 + resourceInitCount2;

            var propScheme = new TestPropScheme
            {

            };
            var resource1 = new Resource(propScheme, resourceInitCount1);
            var resource2 = new Resource(propScheme, resourceInitCount2);

            var propStore = CreatePropStore();



            // ACT
            propStore.Add(resource1);
            propStore.Add(resource2);



            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().HaveCount(expectedItemCount);
            items[0].Scheme.Sid.Should().Be(propScheme.Sid);
            items[0].Should().BeOfType<Resource>();
            ((Resource)items[0]).Count.Should().Be(expectedResourceCount);
        }

        /// <summary>
        /// Тест проверяет, что при добавлении чертежа он помещается в контейнер.
        /// </summary>
        [Test]
        public void Add_Concent_EquipmentInItems()
        {
            // ARRANGE
            const int expectedItemCount = 1;

            var craftPropScheme = new TestPropScheme
            {
            };

            var propScheme = new TestPropScheme
            {
            };
            var concent = new Concept(propScheme, craftPropScheme);

            var propStore = CreatePropStore();



            // ACT
            propStore.Add(concent);



            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().HaveCount(expectedItemCount);
            items[0].Should().BeOfType<Concept>();
            items[0].Should().Be(concent);
        }

        /// <summary>
        /// Тест проверяет, что при удалении экипировки она изымается из контейнера.
        /// </summary>
        [Test]
        public void Remove_Equipment_PropStoreIsEmpty()
        {
            // ARRANGE
            var propScheme = new TestPropScheme
            {
                Equip = new TestPropEquipSubScheme()
            };
            var equipment = new Equipment(propScheme, new ITacticalActScheme[0]);

            var propStore = CreatePropStore();
            propStore.Add(equipment);



            // ACT
            propStore.Remove(equipment);



            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().BeEmpty();
        }

        /// <summary>
        /// Тест проверяет, что при удалении ресурса он изымается из контейнера.
        /// </summary>
        [Test]
        public void Remove_Resource_PropStoreIsEmpty()
        {
            // ARRANGE
            const int resourceInitCount = 1;

            var propScheme = new TestPropScheme
            {
            };
            var resource = new Resource(propScheme, resourceInitCount);

            var propStore = CreatePropStore();
            propStore.Add(resource);



            // ACT
            propStore.Remove(resource);



            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().BeEmpty();
        }

        /// <summary>
        /// Тест проверяет, что при удалении ресурса, если удалёется меньше,
        /// чем есть, то ресурс остаётся.
        /// </summary>
        [Test]
        public void Remove_ResourceStack_PropStoreIsEmpty()
        {
            // ARRANGE
            const int resourceInitCount = 3;
            const int resourceToRemoveCount = 1;
            const int expectedItemCount = 1;
            const int expectedResourceCount = resourceInitCount - resourceToRemoveCount;

            var propScheme = new TestPropScheme
            {
            };
            var resource = new Resource(propScheme, resourceInitCount);

            var propStore = CreatePropStore();
            propStore.Add(resource);

            var resourceToRemove = new Resource(propScheme, resourceToRemoveCount);



            // ACT
            propStore.Remove(resourceToRemove);



            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().HaveCount(expectedItemCount);
            ((Resource)items[0]).Count.Should().Be(expectedResourceCount);
        }

        private static PropStoreBase CreatePropStore()
        {
            var propStoreMock = new Mock<PropStoreBase>
            {
                CallBase = true
            };

            var propStore = propStoreMock.Object;
            return propStore;
        }
    }
}