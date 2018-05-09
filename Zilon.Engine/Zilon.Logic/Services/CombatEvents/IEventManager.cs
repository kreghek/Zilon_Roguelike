using System;
using Zilon.Logic.Tactics.Events;

namespace Zilon.Logic.Services.CombatEvents
{
    public interface IEventManager
    {
        event EventHandler<CombatEventArgs> OnEventProcessed;

        void SetEvents(ICommandEvent[] events);
        void EventsToQueue(ICommandEvent[] events);
        void Update();
    }
}