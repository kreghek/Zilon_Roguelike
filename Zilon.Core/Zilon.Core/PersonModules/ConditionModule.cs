using System;
using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public class ConditionModule : IConditionModule
    {
        private readonly List<IPersonEffect> _items;

        public ConditionModule()
        {
            _items = new List<IPersonEffect>();
            IsActive = true;
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

        private void Сondition_Changed(object sender, EventArgs e)
        {
            var effect = (IPersonEffect)sender;
            DoChanged(effect);
        }

        public IEnumerable<IPersonEffect> Items => _items;
        public string Key => nameof(IConditionModule);
        public bool IsActive { get; set; }

        public void Add(IPersonEffect effect)
        {
            if (effect is null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            _items.Add(effect);

            effect.Changed += Сondition_Changed;
            DoAdd(effect);
        }

        public void Remove(IPersonEffect effect)
        {
            if (effect is null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            _items.Remove(effect);
            effect.Changed -= Сondition_Changed;
            DoRemoved(effect);
        }

        public event EventHandler<EffectEventArgs>? Added;
        public event EventHandler<EffectEventArgs>? Removed;
        public event EventHandler<EffectEventArgs>? Changed;
    }
}