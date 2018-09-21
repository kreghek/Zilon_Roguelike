using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Tactics;

namespace Zilon.Core.Tests.Tactics
{
    [TestFixture]
    public class ActorManagerTests
    {
        /// <summary>
        /// Тест проверяет, что менеджер выбрасывает событие, если добавлен один актёр.
        /// </summary>
        [Test]
        public void Add_OneActor_EventRaise()
        {
            // ARRANGE

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var actorManager = new ActorManager();



            // ACT
            using (var monitor = actorManager.Monitor())
            {
                actorManager.Add(actor);



                // ASSERT
                monitor.Should().Raise(nameof(ActorManager.Added))
                    .WithArgs<ManagerItemsChangedArgs<IActor>>((e) => e.Items.Length == 1 && e.Items[0] == actor);
            }
        }

        /// <summary>
        /// Тест проверяет, что менеджер выбрасывает событие, если добавлено несколько актёров.
        /// </summary>
        [Test]
        public void Add_MultipleActors_EventRaise()
        {
            // ARRANGE

            const int actorCount = 3;
            var actorList = new List<IActor>();

            for (var i = 0; i < 3; i++)
            {
                var actorMock = new Mock<IActor>();
                var actor = actorMock.Object;
                actorList.Add(actor);
            }

            var actorManager = new ActorManager();



            // ACT
            using (var monitor = actorManager.Monitor())
            {
                actorManager.Add(actorList);



                // ASSERT
                monitor.Should().Raise(nameof(ActorManager.Added))
                    .WithArgs<ManagerItemsChangedArgs<IActor>>(e => CheckEventArgs(e, actorCount, actorList));
            }
        }

        /// <summary>
        /// Тест проверяет, что менеджер выбрасывает событие, если удалён один актёр.
        /// </summary>
        [Test]
        public void Remove_OneActor_EventRaise()
        {
            // ARRANGE

            var actorMock = new Mock<IActor>();
            var actor = actorMock.Object;

            var actorManager = new ActorManager();
            actorManager.Add(actor);



            // ACT
            using (var monitor = actorManager.Monitor())
            {
                actorManager.Remove(actor);



                // ASSERT
                monitor.Should().Raise(nameof(ActorManager.Removed))
                    .WithArgs<ManagerItemsChangedArgs<IActor>>((e) => e.Items.Length == 1 && e.Items[0] == actor);
            }
        }

        /// <summary>
        /// Тест проверяет, что менеджер выбрасывает событие, если удалено несколько актёров.
        /// </summary>
        [Test]
        public void Remove_MultipleActors_EventRaise()
        {
            // ARRANGE

            const int actorCount = 3;
            var actorList = new List<IActor>();

            for (var i = 0; i < 3; i++)
            {
                var actorMock = new Mock<IActor>();
                var actor = actorMock.Object;
                actorList.Add(actor);
            }

            var actorManager = new ActorManager();
            actorManager.Add(actorList);



            // ACT
            using (var monitor = actorManager.Monitor())
            {
                actorManager.Remove(actorList);



                // ASSERT
                monitor.Should().Raise(nameof(ActorManager.Removed))
                    .WithArgs<ManagerItemsChangedArgs<IActor>>(e => CheckEventArgs(e, actorCount, actorList));
            }
        }

        private bool CheckEventArgs(ManagerItemsChangedArgs<IActor> e, int actorCount, IList<IActor> actorList)
        {
            var isCountEquals = e.Items.Length == actorCount;
            var actorsInEventItems = actorList.All(actor => e.Items.Contains(actor));
            return isCountEquals && actorsInEventItems;
        }
    }
}