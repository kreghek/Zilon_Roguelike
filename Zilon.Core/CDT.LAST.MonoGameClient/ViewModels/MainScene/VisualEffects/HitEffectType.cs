using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    [Flags]
    internal enum HitEffectType
    {
        None,
        ShortBlade = 1,
        Teeth = 2,

        Backing = 1024
    }
}