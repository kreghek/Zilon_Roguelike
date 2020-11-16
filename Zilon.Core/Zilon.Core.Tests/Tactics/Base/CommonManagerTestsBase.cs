using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Tactics;

namespace Zilon.Core.Tests.Tactics.Base
{
    /// <summary>
    /// Базовые тесты для всех менеджеров сущностей сектора.
    /// </summary>
    /// <typeparam name="TSectorEntity"> Тип сущности сектора. </typeparam>
    /// <remarks>
    /// Проверяет выстреливание событий и корректность аргументов событий при добавлении и удалении сущностей.
    /// </remarks>
    public abstract class CommonManagerTestsBase<TSectorEntity> where TSectorEntity : class
    {
        /// <summary>
        /// Тест проверяет, что менеджер выбрасывает событие, если добавлен один актёр.
        /// </summary>
        [Test]
        public void Add_OneEntity_EventRaise()
        {
            // ARRANGE

            var entity = CreateEntity();

            var manager = CreateManager();

            // ACT
            using (var monitor = manager.Monitor())
            {
                manager.Add(entity);

                // ASSERT
                monitor.Should().Raise(nameof(ISectorEntityManager<TSectorEntity>.Added))
                    .WithArgs<ManagerItemsChangedEventArgs<TSectorEntity>>(e =>
                        (e.Items.Length == 1) && (e.Items[0] == entity));
            }
        }

        /// <summary>
        /// Тест проверяет, что менеджер выбрасывает событие, если добавлено несколько сущностей.
        /// </summary>
        [Test]
        public void Add_MultipleEntities_EventRaise()
        {
            // ARRANGE

            const int entityCount = 3;
            var entityList = new List<TSectorEntity>();

            for (var i = 0; i < 3; i++)
            {
                var entity = CreateEntity();
                entityList.Add(entity);
            }

            var manager = CreateManager();

            // ACT
            using (var monitor = manager.Monitor())
            {
                manager.Add(entityList);

                // ASSERT
                monitor.Should().Raise(nameof(ISectorEntityManager<TSectorEntity>.Added))
                    .WithArgs<ManagerItemsChangedEventArgs<TSectorEntity>>(e =>
                        CheckEventArgs(e, entityCount, entityList));
            }
        }

        /// <summary>
        /// Тест проверяет, что менеджер выбрасывает событие, если удалена одна сущность.
        /// </summary>
        [Test]
        public void Remove_OneEntity_EventRaise()
        {
            // ARRANGE

            var entity = CreateEntity();

            var manager = CreateManager();
            manager.Add(entity);

            // ACT
            using (var monitor = manager.Monitor())
            {
                manager.Remove(entity);

                // ASSERT
                monitor.Should().Raise(nameof(ISectorEntityManager<TSectorEntity>.Removed))
                    .WithArgs<ManagerItemsChangedEventArgs<TSectorEntity>>(e =>
                        (e.Items.Length == 1) && (e.Items[0] == entity));
            }
        }

        /// <summary>
        /// Тест проверяет, что менеджер выбрасывает событие, если удалено несколько сущностей.
        /// </summary>
        [Test]
        public void Remove_MultipleEntities_EventRaise()
        {
            // ARRANGE

            const int entityCount = 3;
            var entityList = new List<TSectorEntity>();

            for (var i = 0; i < 3; i++)
            {
                var entity = CreateEntity();
                entityList.Add(entity);
            }

            var manager = CreateManager();
            manager.Add(entityList);

            // ACT
            using (var monitor = manager.Monitor())
            {
                manager.Remove(entityList);

                // ASSERT
                monitor.Should().Raise(nameof(ISectorEntityManager<TSectorEntity>.Removed))
                    .WithArgs<ManagerItemsChangedEventArgs<TSectorEntity>>(e =>
                        CheckEventArgs(e, entityCount, entityList));
            }
        }

        /// <summary>
        /// Создание тестируемого менеджера сущностей сектора. В конкретных тестах будет реализовываться.
        /// </summary>
        /// <returns> Возвращает экземпляр конкретного менеджера сущностей сектора. </returns>
        protected abstract ISectorEntityManager<TSectorEntity> CreateManager();

        private bool CheckEventArgs(
            ManagerItemsChangedEventArgs<TSectorEntity> e,
            int actorCount,
            IList<TSectorEntity> actorList)
        {
            var isCountEquals = e.Items.Length == actorCount;
            var actorsInEventItems = actorList.All(actor => e.Items.Contains(actor));
            return isCountEquals && actorsInEventItems;
        }

        private static TSectorEntity CreateEntity()
        {
            var entityMock = new Mock<TSectorEntity>();
            var entity = entityMock.Object;
            return entity;
        }
    }
}