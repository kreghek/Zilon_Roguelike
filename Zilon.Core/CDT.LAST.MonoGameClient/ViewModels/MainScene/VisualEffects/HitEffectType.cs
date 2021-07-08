using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    [Flags]
    internal enum HitEffectType
    {
        None,
        ShortBlade = 1,

        Backing = 1024
    }
}