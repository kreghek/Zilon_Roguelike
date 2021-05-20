﻿using System;
using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public interface IConditionModule : IPersonModule
    {
        IEnumerable<IPersonEffect> Items { get; }

        void Add(IPersonEffect effect);
        void Remove(IPersonEffect effect);

        event EventHandler<EffectEventArgs>? Added;
        event EventHandler<EffectEventArgs>? Changed;
        event EventHandler<EffectEventArgs>? Removed;
    }
}