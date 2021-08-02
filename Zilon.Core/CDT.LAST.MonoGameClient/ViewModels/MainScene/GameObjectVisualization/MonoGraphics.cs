using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    public sealed class MonoGraphics : SpriteContainer, IActorGraphics
    {
        private readonly SpriteContainer _hitlighted;
        private readonly SpriteContainer _outline;

        public MonoGraphics(string sid, IPersonVisualizationContentStorage personVisualizationContentStorage)
        {
            var outlinedTexture = personVisualizationContentStorage.GetMonographicTexture($"{sid}/Outlined");

            var spritePosition = GetSpritePosition(sid);

            _outline = CreateSpriteHierarchy(outlinedTexture, spritePosition);
            _outline.Color = LastColors.Red;
            AddChild(_outline);

            var mainTexture = personVisualizationContentStorage.GetMonographicTexture(sid);

            var main = CreateSpriteHierarchy(mainTexture, spritePosition);
            AddChild(main);

            var hitlightedTexture = personVisualizationContentStorage.GetMonographicTexture($"{sid}/Outlined");

            _hitlighted = CreateSpriteHierarchy(hitlightedTexture, spritePosition);
            _hitlighted.Color = LastColors.Red;
            AddChild(_hitlighted);
        }

        protected override void DoDraw(SpriteBatch spriteBatch, float zindex)
        {
            _outline.Visible = ShowOutlined;
            _hitlighted.Visible = ShowHitlighted;

            base.DoDraw(spriteBatch, zindex);
        }

        private static Vector2 GetSpritePosition(string sid)
        {
            return sid switch
            {
                "predator" or "predator-meat" => new Vector2(0, -12),
                "warthog" => new Vector2(-5, -12),
                "skeleton" or "skeleton-equipment" => new Vector2(0, -22),
                "gallbladder" => new Vector2(0, -12),
                _ => new Vector2(0, -12),
            };
        }

        private static SpriteContainer CreateSpriteHierarchy(Texture2D texture2D, Vector2 spritePosition)
        {
            var container = new SpriteContainer();

            container.AddChild(new Sprite(texture2D)
            {
                Position = spritePosition,
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