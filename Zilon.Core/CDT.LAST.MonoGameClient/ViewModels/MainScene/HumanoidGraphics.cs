using System;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class HumanoidGraphics : SpriteContainer, IActorGraphics
    {
        public HumanoidGraphics(ContentManager contentManager)
        {
            var headTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/head");
            var bodyTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/body");
            var legsTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/legs-idle");
            var armLeftTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/arm-left-simple");
            var armRightTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/arm-right-simple");

            var shortSwordBaseTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/ShortSwordBase");

            /*var bodySpriteContainer = new SpriteContainer();
            AddChild(bodySpriteContainer);

            var armLeftSprite = new Sprite(armLeftTexture)
            {
                Position = new Vector2(2, -29),
                Origin = new Vector2(11f / 16, 4f / 24),
            };
            bodySpriteContainer.AddChild(armLeftSprite);

            var bodySprite = new Sprite(bodyTexture)
            {
                Position = new Vector2(0, -12),
                Origin = new Vector2(10f / 32, 24f / 32),
            };
            bodySpriteContainer.AddChild(bodySprite);

            var headSprite = new Sprite(headTexture)
            {
                Position = new Vector2(2, -29),
                Origin = new Vector2(15f / 32, 20f / 32),
            };
            bodySpriteContainer.AddChild(headSprite);*/

            AddChild(new Sprite(armLeftTexture)
            {
                Position = new Vector2(-10, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(shortSwordBaseTexture)
            {
                Position = new Vector2(-14, -21),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(legsTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.75f)
            });

            AddChild(new Sprite(bodyTexture)
            {
                Position = new Vector2(3, -22),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(headTexture)
            {
                Position = new Vector2(-0, -20),
                Origin = new Vector2(0.5f, 1)
            });

            AddChild(new Sprite2(shortSwordBaseTexture)
            {
                Position = new Vector2(11, -10),
                Origin = new Vector2(0.5f, 0.5f),
                Rotation = (float)(-Math.PI / 2)
            });

            AddChild(new Sprite(armRightTexture)
            {
                Position = new Vector2(13, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });
        }

        public SpriteContainer RootSprite => this;
        public Vector2 HitEffectPosition => Vector2.UnitY * -24;
    }
}