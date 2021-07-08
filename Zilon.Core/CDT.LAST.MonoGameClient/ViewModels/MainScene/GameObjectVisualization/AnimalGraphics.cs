using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    public sealed class AnimalGraphics : SpriteContainer, IActorGraphics
    {
        private readonly SpriteContainer _hitlighted;
        private readonly SpriteContainer _outline;

        public AnimalGraphics(IPersonVisualizationContentStorage personVisualizationContentStorage)
        {
            var outlinedParts = personVisualizationContentStorage.GetAnimalParts("hunter/Outlined");

            _outline = CreateSpriteHierarchy(outlinedParts);
            _outline.Color = Color.Red;
            AddChild(_outline);

            var parts = personVisualizationContentStorage.GetAnimalParts("hunter");

            var main = CreateSpriteHierarchy(parts);
            AddChild(main);

            var hitlightedParts = personVisualizationContentStorage.GetAnimalParts("hunter/Outlined");

            _hitlighted = CreateSpriteHierarchy(hitlightedParts);
            _hitlighted.Color = Color.Red;
            AddChild(_hitlighted);
        }

        protected override void DoDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, float zindex)
        {
            _outline.Visible = ShowOutlined;
            _hitlighted.Visible = ShowHitlighted;

            base.DoDraw(spriteBatch, zindex);
        }

        private static SpriteContainer CreateSpriteHierarchy(IEnumerable<AnimalPart> parts)
        {
            var bodySprite = parts.Single(x => x.Type == AnimalPartType.Body).Texture;
            var headSprite = parts.Single(x => x.Type == AnimalPartType.Head).Texture;
            var tailSprite = parts.Single(x => x.Type == AnimalPartType.Tail).Texture;
            var legCloseFrontSprite = parts.Single(x => x.Type == AnimalPartType.LegCloseFront).Texture;
            var legCloseHindSprite = parts.Single(x => x.Type == AnimalPartType.LegCloseHind).Texture;
            var legFarFrontSprite = parts.Single(x => x.Type == AnimalPartType.LegFarFront).Texture;
            var legFarHindSprite = parts.Single(x => x.Type == AnimalPartType.LegFarHind).Texture;

            var container = new SpriteContainer();

            container.AddChild(new Sprite(legFarFrontSprite)
            {
                Position = new Vector2(0, -12),
                Origin = new Vector2(0.5f, 0.5f)
            });

            container.AddChild(new Sprite(legFarHindSprite)
            {
                Position = new Vector2(0, -12),
                Origin = new Vector2(0.5f, 0.5f)
            });

            container.AddChild(new Sprite(bodySprite)
            {
                Position = new Vector2(0, -12),
                Origin = new Vector2(0.5f, 0.5f)
            });

            container.AddChild(new Sprite(headSprite)
            {
                Position = new Vector2(-0, -12),
                Origin = new Vector2(0.5f, 0.5f)
            });

            container.AddChild(new Sprite(tailSprite)
            {
                Position = new Vector2(0, -12),
                Origin = new Vector2(0.5f, 0.5f)
            });

            container.AddChild(new Sprite(legCloseFrontSprite)
            {
                Position = new Vector2(0, -12),
                Origin = new Vector2(0.5f, 0.5f)
            });

            container.AddChild(new Sprite(legCloseHindSprite)
            {
                Position = new Vector2(0, -12),
                Origin = new Vector2(0.5f, 0.5f)
            });

            return container;
        }

        public SpriteContainer RootSprite => this;
        public Vector2 HitEffectPosition => Vector2.UnitY * -12;

        public bool ShowOutlined { get; set; }
        public bool ShowHitlighted { get; set; }
    }
}