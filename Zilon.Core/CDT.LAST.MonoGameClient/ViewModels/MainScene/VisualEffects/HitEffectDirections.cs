using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    /// <summary>
    /// Possible directions of hit effect sprites.
    /// </summary>
    [Flags]
    internal enum HitEffectDirections
    {
        None,
        Left = 1,
        Top = 2,
        Bottom = 4
    }
}