using System;
using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    public class EffectCollection : IEffectCollection
    {
        private readonly List<IPersonEffect> _items;

        public EffectCollection()
        {
            _items = new List<IPersonEffect>();
        }

        public IEnumerable<IPersonEffect> Items { get => _items; }

        public void Add(IPersonEffect effect)
        {
            if (effect is null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            _items.Add(effect);

            effect.Changed += Effect_Changed;
            DoAdd(effect);
        }

        public void Remove(IPersonEffect effect)
        {
            if (effect is null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            _items.Remove(effect);
            effect.Changed -= Effect_Changed;
            DoRemoved(effect);
        }

        private void Effect_Changed(object sender, EventArgs e)
        {
            var effect = (IPersonEffect)sender;
            DoChanged(effect);
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
