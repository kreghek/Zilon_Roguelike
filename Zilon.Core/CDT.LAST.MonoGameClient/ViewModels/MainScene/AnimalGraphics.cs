﻿using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class AnimalGraphics : SpriteContainer
    {
        public AnimalGraphics(ContentManager contentManager)
        {
            var bodySprite = contentManager.Load<Texture2D>("Sprites/game-objects/hunter/body");
            var headSprite = contentManager.Load<Texture2D>("Sprites/game-objects/hunter/head");
            var tailSprite = contentManager.Load<Texture2D>("Sprites/game-objects/hunter/tail");
            var legCloseFrontSprite = contentManager.Load<Texture2D>("Sprites/game-objects/hunter/leg-close-front");
            var legCloseHindSprite = contentManager.Load<Texture2D>("Sprites/game-objects/hunter/leg-close-hind");
            var legFarFrontSprite = contentManager.Load<Texture2D>("Sprites/game-objects/hunter/leg-far-front");
            var legFarHindSprite = contentManager.Load<Texture2D>("Sprites/game-objects/hunter/leg-far-hind");

            AddChild(new Sprite(legFarFrontSprite)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(legFarHindSprite)
            {
                Position = new Vector2(-10, 0),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(bodySprite)
            {
                Position = new Vector2(0, -8),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(headSprite)
            {
                Position = new Vector2(-8, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(tailSprite)
            {
                Position = new Vector2(8, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(legCloseFrontSprite)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(legCloseHindSprite)
            {
                Position = new Vector2(-10, 0),
                Origin = new Vector2(0.5f, 0.5f)
            });
        }
    }
}