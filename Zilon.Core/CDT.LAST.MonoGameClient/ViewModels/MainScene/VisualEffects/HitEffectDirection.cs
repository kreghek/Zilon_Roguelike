using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    [Flags]
    internal enum HitEffectDirection
    {
        None,
        Left = 1,
        Top = 2,
        Bottom = 4
    }
}