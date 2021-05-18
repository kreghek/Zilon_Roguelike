using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public interface IActorGraphics
    {
        SpriteContainer RootSprite { get; }
        Vector2 HitEffectPosition { get; }
    }
}