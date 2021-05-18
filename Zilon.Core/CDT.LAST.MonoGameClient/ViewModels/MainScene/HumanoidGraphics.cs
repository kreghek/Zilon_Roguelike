using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class HumanoidGraphics : SpriteContainer
    {
        public HumanoidGraphics(ContentManager contentManager)
        {
            var personHeadSprite = contentManager.Load<Texture2D>("Sprites/game-objects/human/head");
            var personBodySprite = contentManager.Load<Texture2D>("Sprites/game-objects/human/body");
            var personLegsSprite = contentManager.Load<Texture2D>("Sprites/game-objects/human/legs-idle");
            var personArmLeftSprite = contentManager.Load<Texture2D>("Sprites/game-objects/human/arm-left-simple");
            var personArmRightSprite = contentManager.Load<Texture2D>("Sprites/game-objects/human/arm-right-simple");

            AddChild(new Sprite(personArmLeftSprite)
            {
                Position = new Vector2(-10, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(personLegsSprite)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.75f)
            });

            AddChild(new Sprite(personBodySprite)
            {
                Position = new Vector2(3, -22),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(personHeadSprite)
            {
                Position = new Vector2(-0, -20),
                Origin = new Vector2(0.5f, 1)
            });

            AddChild(new Sprite(personArmRightSprite)
            {
                Position = new Vector2(13, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });
        }
    }
}