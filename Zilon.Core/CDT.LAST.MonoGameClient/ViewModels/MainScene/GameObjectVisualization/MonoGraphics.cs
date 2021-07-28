using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    public sealed class MonoGraphics : SpriteContainer, IActorGraphics
    {
        private readonly IPersonVisualizationContentStorage _personVisualizationContentStorage;

        private readonly SpriteContainer _hitlighted;
        private readonly SpriteContainer _outline;

        public MonoGraphics(string sid, IPersonVisualizationContentStorage personVisualizationContentStorage)
        {
            var outlinedTexture = personVisualizationContentStorage.GetMonographicTexture($"{sid}/Outlined");

            _outline = CreateSpriteHierarchy(outlinedTexture);
            _outline.Color = LastColors.Red;
            AddChild(_outline);

            var mainTexture = personVisualizationContentStorage.GetMonographicTexture(sid);

            var main = CreateSpriteHierarchy(mainTexture);
            AddChild(main);

            var hitlightedTexture = personVisualizationContentStorage.GetMonographicTexture($"{sid}/Outlined");

            _hitlighted = CreateSpriteHierarchy(hitlightedTexture);
            _hitlighted.Color = LastColors.Red;
            AddChild(_hitlighted);
        }

        private static SpriteContainer CreateSpriteHierarchy(Texture2D texture2D)
        {
            var container = new SpriteContainer();

            container.AddChild(new Sprite(texture2D)
            {
                Position = new Vector2(0, -12),
                Origin = new Vector2(0.5f, 0.5f)
            });

            return container;
        }

        public SpriteContainer RootSprite => this;
        public Vector2 HitEffectPosition => Vector2.UnitY * -12;
        public bool ShowHitlighted { get; set; }
        public bool ShowOutlined { get; set; }
    }
}