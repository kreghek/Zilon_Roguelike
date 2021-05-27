using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class StaticObjectGraphics : SpriteContainer
    {
        public StaticObjectGraphics(Game game)
        {
            var staticObjectTexture = game.Content.Load<Texture2D>("Sprites/game-objects/environment/Grass");
            var staticObjectSprite = new Sprite(staticObjectTexture)
            {
                Origin = new Vector2(0.5f, 0.75f)
            };

            AddChild(staticObjectSprite);
        }
    }
}