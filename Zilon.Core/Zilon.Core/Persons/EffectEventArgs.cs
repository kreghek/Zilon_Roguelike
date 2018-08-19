using System;

namespace Zilon.Core.Persons
{
    public sealed class EffectEventArgs : EventArgs
    {
        public IPersonEffect Effect { get; }

        public EffectEventArgs(IPersonEffect effect)
        {
            Effect = effect;
        }
    }
}
