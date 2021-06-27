using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectGraphics
{
    public interface IActorGraphics
    {
        Vector2 HitEffectPosition { get; }
        SpriteContainer RootSprite { get; }
        bool ShowOutlined { get; set; }
    }
}