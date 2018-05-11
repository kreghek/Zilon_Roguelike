namespace Zilon.Core.Tests.Services.CombatEvents
{
    using FluentAssertions;

    using Moq;

    using NUnit.Framework;

    using Zilon.Core.Services.CombatEvents;
    using Zilon.Core.Tactics.Events;

    [TestFixture]
    public class EventManagerTests
    {
        /// <summary>
        /// 1. В системе есть событие на команду с нулевым триггером. Событие добавлено в менеджера.
        /// 2. Выполняем итерацию менеджера.
        /// 3. Менеджер сообщил, что событие произошло.
        /// </summary>
        [Test]
        public void Update_Default_EventRaised()
        {
            // ARRANGE
            var eventManager = new EventManager();
            var fakeEventMock = new Mock<ITacticEvent>();
            var fakeEvent = fakeEventMock.Object;
            var events = new[] { fakeEvent };
            eventManager.SetEvents(events);


            using (var monitor = eventManager.Monitor())
            {
                // ACT
                eventManager.Update();



                // ASSERT
            
                monitor.Should().Raise(nameof(eventManager.OnEventProcessed))
                    .WithArgs<CombatEventArgs>(x=>x.CommandEvent == fakeEvent);
            }
        }
    }
}