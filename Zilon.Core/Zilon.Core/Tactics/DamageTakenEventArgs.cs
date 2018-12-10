using System;

namespace Zilon.Core.Tactics
{
    public sealed class DamageTakenEventArgs: EventArgs
    {
        public DamageTakenEventArgs(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
