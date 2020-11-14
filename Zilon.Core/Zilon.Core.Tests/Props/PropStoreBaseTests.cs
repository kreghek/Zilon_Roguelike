using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Props
{
    [TestFixture]
    public class PropStoreBaseTests
    {
        private TestPropScheme _conceptScheme;
        private TestPropScheme _equipmentScheme;
        private TestPropScheme _resourceScheme;

        /// <summary>
        /// Тест проверяет, что при добавлении экипировки она помещается в контейнер.
        /// </summary>
        [Test]
        public void Add_Equipment_EquipmentInItems()
        {
            // ARRANGE
            const int expectedItemCount = 1;

            var equipment = CreateEquipment();

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

            var resource = CreateResource(resourceInitCount);

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

            var resource1 = CreateResource(resourceInitCount1);
            var resource2 = CreateResource(resourceInitCount2);

            var propStore = CreatePropStore();

            // ACT
            propStore.Add(resource1);
            propStore.Add(resource2);

            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().HaveCount(expectedItemCount);
            items[0].Scheme.Sid.Should().Be(_resourceScheme.Sid);
            items[0].Should().BeOfType<Resource>();
            ((Resource)items[0]).Count.Should().Be(expectedResourceCount);
        }

        /// <summary>
        /// Тест проверяет, что при добавлении чертежа он помещается в контейнер.
        /// </summary>
        [Test]
        public void Add_Concent_ConceptInItems()
        {
            // ARRANGE
            const int expectedItemCount = 1;

            var concent = CreateConcept();

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
            var equipment = CreateEquipment();

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

            var resource = CreateResource(resourceInitCount);

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

            var resource = CreateResource(resourceInitCount);

            var propStore = CreatePropStore();
            propStore.Add(resource);

            var resourceToRemove = CreateResource(resourceToRemoveCount);

            // ACT
            propStore.Remove(resourceToRemove);

            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().HaveCount(expectedItemCount);
            ((Resource)items[0]).Count.Should().Be(expectedResourceCount);
        }

        /// <summary>
        /// Тест проверяет, что при удалении чертежа хранилище остаётся пустым.
        /// </summary>
        [Test]
        public void Remove_Concent_PropStoreIsEmpty()
        {
            // ARRANGE
            var concent = CreateConcept();

            var propStore = CreatePropStore();

            propStore.Add(concent);

            // ACT
            propStore.Remove(concent);

            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().BeEmpty();
        }

        /// <summary>
        /// Тест проверяет, что при добавлении экипировки выстреливает событие на добаление.
        /// </summary>
        [Test]
        public void Add_Equipment_EventRaise()
        {
            // ARRANGE

            var equipment = CreateEquipment();

            var propStore = CreatePropStore();

            using (var monitor = propStore.Monitor())
            {
                // ACT
                propStore.Add(equipment);

                // ASSERT
                monitor.Should().Raise(nameof(IPropStore.Added));
            }
        }

        /// <summary>
        /// Тест проверяет, что при добавлении чертежа выстреливает событие на добавление.
        /// </summary>
        [Test]
        public void Add_Concent_EventRaise()
        {
            // ARRANGE
            var concent = CreateConcept();

            var propStore = CreatePropStore();

            using (var monitor = propStore.Monitor())
            {
                // ACT
                propStore.Add(concent);

                // ASSERT
                monitor.Should().Raise(nameof(IPropStore.Added));
            }
        }

        /// <summary>
        /// Тест проверяет, что при добавлении экипировки выстреливает событие на добаление.
        /// </summary>
        [Test]
        public void Add_Resource_EventRaise()
        {
            // ARRANGE
            const int resourceInitCount = 3;

            var resource = CreateResource(resourceInitCount);

            var propStore = CreatePropStore();

            using (var monitor = propStore.Monitor())
            {
                // ACT
                propStore.Add(resource);

                // ASSERT
                monitor.Should().Raise(nameof(IPropStore.Added));
            }
        }

        /// <summary>
        /// Тест проверяет, что при удалении экипировки выстреивает событие на удаление.
        /// </summary>
        [Test]
        public void Remove_Equipment_EventRaise()
        {
            // ARRANGE
            var equipment = CreateEquipment();

            var propStore = CreatePropStore();
            propStore.Add(equipment);

            using (var monitor = propStore.Monitor())
            {
                // ACT
                propStore.Remove(equipment);

                // ASSERT
                monitor.Should().Raise(nameof(IPropStore.Removed));
            }
        }

        /// <summary>
        /// Тест проверяет, что при удалении ресурса выстреивает событие на удаление.
        /// </summary>
        [Test]
        public void Remove_Resource_EventRaise()
        {
            // ARRANGE
            const int resourceInitCount = 1;
            var resource = CreateResource(resourceInitCount);

            var propStore = CreatePropStore();
            propStore.Add(resource);

            using (var monitor = propStore.Monitor())
            {
                // ACT
                propStore.Remove(resource);

                // ASSERT
                monitor.Should().Raise(nameof(IPropStore.Removed));
            }
        }

        /// <summary>
        /// Тест проверяет, что при удалении ресурса, если стак ещё остаётся, выстреивает событие на изменение.
        /// </summary>
        [Test]
        public void Remove_Resource_ChangedEventRaise()
        {
            // ARRANGE
            const int resourceInitCount = 2;
            const int resourceTakenCount = 1;
            var resource = CreateResource(resourceInitCount);

            var propStore = CreatePropStore();
            propStore.Add(resource);

            var usedResource = CreateResource(resourceTakenCount);

            using (var monitor = propStore.Monitor())
            {
                // ACT
                propStore.Remove(usedResource);

                // ASSERT
                monitor.Should().Raise(nameof(IPropStore.Changed));
            }
        }

        /// <summary>
        /// Тест проверяет, что при удалении чертежа хранилище остаётся пустым.
        /// </summary>
        [Test]
        public void Remove_Concent_EventRaise()
        {
            // ARRANGE
            var concent = CreateConcept();

            var propStore = CreatePropStore();

            propStore.Add(concent);

            // ACT
            using (var monitor = propStore.Monitor())
            {
                // ACT
                propStore.Remove(concent);

                // ASSERT
                monitor.Should().Raise(nameof(IPropStore.Removed));
            }

            // ASSERT
            var items = propStore.CalcActualItems();
            items.Should().BeEmpty();
        }

        [SetUp]
        public void SetUp()
        {
            _equipmentScheme = new TestPropScheme
            {
                Sid = "equipment",
                Name = new LocalizedStringSubScheme
                {
                    Ru = "Тестовая экипировка"
                },
                Equip = new TestPropEquipSubScheme()
            };

            _resourceScheme = new TestPropScheme
            {
                Sid = "resource",
                Name = new LocalizedStringSubScheme
                {
                    Ru = "Тестовый ресурс"
                }
            };

            _conceptScheme = new TestPropScheme
            {
                Sid = "concept",
                Name = new LocalizedStringSubScheme
                {
                    Ru = "Тестовый чертёж"
                }
            };
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

        private Equipment CreateEquipment()
        {
            var equipment = new Equipment(_equipmentScheme, new ITacticalActScheme[0]);
            return equipment;
        }

        private Resource CreateResource(int resourceInitCount)
        {
            var resource = new Resource(_resourceScheme, resourceInitCount);
            return resource;
        }

        private Concept CreateConcept()
        {
            var concent = new Concept(_conceptScheme, _equipmentScheme);

            return concent;
        }
    }
}