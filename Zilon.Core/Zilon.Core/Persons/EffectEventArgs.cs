using System;

namespace Zilon.Core.Persons
{
    public sealed class EffectEventArgs : EventArgs
    {
        public EffectEventArgs(IPersonCondition effect)
        {
            Effect = effect;
        }

        public IPersonCondition Effect { get; }
    }
}