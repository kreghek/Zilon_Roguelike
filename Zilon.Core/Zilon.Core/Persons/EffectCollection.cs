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
            DoAdd(effect);
        }

        private void Effect_Changed(object sender, EventArgs e)
        {
            var effect = sender as IPersonEffect;
            DoChanged(effect);
        }

        public void Remove(IPersonEffect effect)
        {
            Items.Remove(effect);
            effect.Changed -= Effect_Changed;
            DoRemoved(effect);
        }

        private void DoAdd(IPersonEffect effect)
        {
            var args = new EffectEventArgs(effect);
            Added?.Invoke(this, args);
        }

        private void DoChanged(IPersonEffect effect)
        {
            var args = new EffectEventArgs(effect);
            Changed?.Invoke(this, args);
        }

        private void DoRemoved(IPersonEffect effect)
        {
            var args = new EffectEventArgs(effect);
            Removed?.Invoke(this, args);
        }

        public event EventHandler<EffectEventArgs> Added;
        public event EventHandler<EffectEventArgs> Removed;
        public event EventHandler<EffectEventArgs> Changed;
    }
}
