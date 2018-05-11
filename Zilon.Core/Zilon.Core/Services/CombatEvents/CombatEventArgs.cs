using System;
using Zilon.Logic.Tactics.Events;

namespace Zilon.Logic.Services.CombatEvents
{
    public class CombatEventArgs : EventArgs
    {
        public ICommandEvent CommandEvent { get; set; }
        public event EventHandler<EventHandler> OnComplete;
    }
}
