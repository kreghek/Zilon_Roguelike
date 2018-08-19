using System;

namespace Zilon.Core.Persons
{
    public interface IPersonEffect
    {
        EffectRule[] Rules { get; }

        void Update();

        event EventHandler Changed;
    }
}
