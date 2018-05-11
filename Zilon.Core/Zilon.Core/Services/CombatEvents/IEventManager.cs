namespace Zilon.Core.Services.CombatEvents
{
    using System;

    using Zilon.Core.Tactics.Events;

    public interface IEventManager
    {
        event EventHandler<CombatEventArgs> OnEventProcessed;

        void SetEvents(ICommandEvent[] events);
        void EventsToQueue(ICommandEvent[] events);
        void Update();
    }
}