using System;
using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    public class EffectCollection
    {
        public EffectCollection()
        {
            Items = new List<IPersonEffect>();
        }

        public IList<IPersonEffect> Items { get; }

        public void Add(IPersonEffect effect)
        {
            Items.Add(effect);

            var args = new EffectEventArgs(effect);
            Added?.Invoke(this, args);
        }

        public void Remove(IPersonEffect effect)
        {
            Items.Remove(effect);

            var args = new EffectEventArgs(effect);
            Removed?.Invoke(this, args);
        }

        public event EventHandler<EffectEventArgs> Added;
        public event EventHandler<EffectEventArgs> Removed;
    }
}
