using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Logic.Tactics.Events;

namespace Zilon.Logic.Services.CombatEvents
{
    public class EventManager
    {
        private readonly Dictionary<string, List<ICommandEvent>> eventDict;
        private readonly List<ICommandEvent> launchEvents;
        private readonly List<ICommandEvent> waitingEvents;
        private bool processingBegins;

        public event EventHandler<CombatEventArgs> OnEventProcessed;


        public EventManager()
        {
            eventDict = new Dictionary<string, List<ICommandEvent>>();
            launchEvents = new List<ICommandEvent>();
            waitingEvents = new List<ICommandEvent>();
        }

        public void SetEvents(ICommandEvent[] events)
        {
            eventDict.Clear();
            launchEvents.Clear();
            processingBegins = true;
            EventsToQueue(events);
        }

        public void LaunchTargetEvents(ICommandEvent targetEvent, string[] names)
        {
        }

        public void Update()
        {
            while (launchEvents.Any())
            {
                var combatEvent = launchEvents.First();
                waitingEvents.Add(combatEvent);
                launchEvents.RemoveAt(0);
                ProcessEvent(combatEvent);
            }
        }

        public void ProcessEvent(ICommandEvent targetEvent)
        {
            var args = new CombatEventArgs
            {
                CommandEvent = targetEvent
            };

            args.OnComplete += (s, e)=> {
                ComplateEvent(targetEvent);
            };

            OnEventProcessed?.Invoke(this, args);
        }

        private void ComplateEvent(ICommandEvent targetEvent)
        {
            throw new NotImplementedException();
        }

        private void EventsToQueue(ICommandEvent[] events)
        {
            foreach (var combatEvent in events)
            {
                if (combatEvent.TriggerName != null)
                {
                    if (!eventDict.ContainsKey(combatEvent.TriggerName))
                    {
                        eventDict.Add(combatEvent.TriggerName, new List<ICommandEvent>());
                    }

                    eventDict[combatEvent.TriggerName].Add(combatEvent);
                }
                else
                {
                    launchEvents.Add(combatEvent);
                }
            }
        }
    }
}
