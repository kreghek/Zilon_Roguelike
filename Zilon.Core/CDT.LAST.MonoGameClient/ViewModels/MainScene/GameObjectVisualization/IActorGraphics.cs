using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    public interface IActorGraphics
    {
        Vector2 HitEffectPosition { get; }
        SpriteContainer RootSprite { get; }
        bool ShowHitlighted { get; set; }
        bool ShowOutlined { get; set; }
    }
}