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

            effect.Changed += Effect_Changed;

            var args = new EffectEventArgs(effect);
            Added?.Invoke(this, args);
        }

        private void Effect_Changed(object sender, EventArgs e)
        {
            var effect = sender as IPersonEffect;
            var args = new EffectEventArgs(effect);
            Changed?.Invoke(this, args);
        }

        public void Remove(IPersonEffect effect)
        {
            Items.Remove(effect);
            effect.Changed -= Effect_Changed;

            var args = new EffectEventArgs(effect);
            Removed?.Invoke(this, args);
        }

        public event EventHandler<EffectEventArgs> Added;
        public event EventHandler<EffectEventArgs> Removed;
        public event EventHandler<EffectEventArgs> Changed;
    }
}
