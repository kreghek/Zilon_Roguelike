using System;
using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public interface IConditionsModule : IPersonModule
    {
        IEnumerable<IPersonCondition> Items { get; }

        void Add(IPersonCondition condition);
        void Remove(IPersonCondition condition);

        event EventHandler<EffectEventArgs>? Added;
        event EventHandler<EffectEventArgs>? Changed;
        event EventHandler<EffectEventArgs>? Removed;
    }
}