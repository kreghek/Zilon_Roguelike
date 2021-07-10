using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Possible types of hit effect sprites.
    /// </summary>
    [Flags]
    internal enum HitEffectTypes
    {
        None,
        ShortBlade = 1,
        Teeth = 2,

        Backing = 1024
    }
}