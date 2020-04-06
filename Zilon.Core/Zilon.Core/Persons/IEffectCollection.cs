using System;
using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    public interface IEffectCollection
    {
        IEnumerable<IPersonEffect> Items { get; }

        event EventHandler<EffectEventArgs> Added;
        event EventHandler<EffectEventArgs> Changed;
        event EventHandler<EffectEventArgs> Removed;

        void Add(IPersonEffect effect);
        void Remove(IPersonEffect effect);
    }
}