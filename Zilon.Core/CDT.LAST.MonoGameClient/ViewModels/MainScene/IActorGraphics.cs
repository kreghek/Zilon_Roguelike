using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public interface IActorGraphics
    {
        Vector2 HitEffectPosition { get; }
        SpriteContainer RootSprite { get; }
    }
}