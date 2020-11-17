using System;

namespace Zilon.Core.Persons
{
    public sealed class EffectEventArgs : EventArgs
    {
        public EffectEventArgs(IPersonEffect effect)
        {
            Effect = effect;
        }

        public IPersonEffect Effect { get; }
    }
}