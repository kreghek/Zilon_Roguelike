using System;
using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    public class ConditionsModule : IConditionsModule
    {
        private readonly List<IPersonCondition> _items;

        public ConditionsModule()
        {
            _items = new List<IPersonCondition>();
            IsActive = true;
        }

        private void DoAdd(IPersonCondition effect)
        {
            var args = new EffectEventArgs(effect);
            Added?.Invoke(this, args);
        }

        private void DoChanged(IPersonCondition effect)
        {
            var args = new EffectEventArgs(effect);
            Changed?.Invoke(this, args);
        }

        private void DoRemoved(IPersonCondition effect)
        {
            var args = new EffectEventArgs(effect);
            Removed?.Invoke(this, args);
        }

        private void Сondition_Changed(object? sender, EventArgs e)
        {
            if (sender is null)
            {
                throw new InvalidOperationException("Sender could not be null.");
            }

            var effect = (IPersonCondition)sender;

            DoChanged(effect);
        }

        public IEnumerable<IPersonCondition> Items => _items;
        public string Key => nameof(IConditionsModule);
        public bool IsActive { get; set; }

        public void Add(IPersonCondition condition)
        {
            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            _items.Add(condition);

            condition.Changed += Сondition_Changed;
            DoAdd(condition);
        }

        public void Remove(IPersonCondition condition)
        {
            _items.Remove(condition);
            condition.Changed -= Сondition_Changed;
            DoRemoved(condition);
        }

        public event EventHandler<EffectEventArgs>? Added;
        public event EventHandler<EffectEventArgs>? Removed;
        public event EventHandler<EffectEventArgs>? Changed;
    }
}