using System;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class StaticObjectViewModel : GameObjectBase, IContainerViewModel
    {
        private const int HIGHLIGHT_DURATION_SECONDS = 3;
        private readonly Game _game;
        private readonly SpriteContainer _rootSprite;
        private readonly SpriteBatch _spriteBatch;

        private double? _highlightCounter;

        public StaticObjectViewModel(Game game, IStaticObject staticObject, SpriteBatch spriteBatch, bool createHighlighted = false)
        {
            _game = game;
            StaticObject = staticObject;
            _spriteBatch = spriteBatch;

            var graphics = new StaticObjectGraphics(game, staticObject);

            var worldCoords = HexHelper.ConvertToWorld(((HexNode)StaticObject.Node).OffsetCoords);

            var hexSize = MapMetrics.UnitSize / 2;
            var staticObjectPosition = new Vector2(
                (int)Math.Round(worldCoords[0] * hexSize * Math.Sqrt(HIGHLIGHT_DURATION_SECONDS), MidpointRounding.ToEven),
                (int)Math.Round(worldCoords[1] * hexSize * 2 / 2, MidpointRounding.ToEven)
            );

            _rootSprite = new SpriteContainer
            {
                Position = staticObjectPosition
            };

            var shadowTexture = _game.Content.Load<Texture2D>("Sprites/game-objects/simple-object-shadow");
            _rootSprite.AddChild(new Sprite(shadowTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(Color.White, 0.5f)
            });

            _rootSprite.AddChild(graphics);

            if (createHighlighted)
            {
                _highlightCounter = HIGHLIGHT_DURATION_SECONDS;
            }
        }

        public override bool HiddenByFow => false;

        public override Vector2 HitEffectPosition => Vector2.UnitY * -24;
        public override IGraphNode Node => StaticObject.Node;

        public override void Draw(GameTime gameTime, Matrix transform)
        {
            _spriteBatch.Begin(transformMatrix: transform, blendState: BlendState.AlphaBlend);

            if (_highlightCounter is not null)
            {
                var t = _highlightCounter.Value / HIGHLIGHT_DURATION_SECONDS;
                var t2 = Math.Sin(t * HIGHLIGHT_DURATION_SECONDS * Math.PI * 2);
                _rootSprite.Color = Color.Lerp(Color.White, Color.Red, (float)t2);
            }
            else
            {
                _rootSprite.Color = UnderFog ? Color.White * 0.5f : Color.White;
            }

            _rootSprite.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            //_rootSprite.Color = UnderFog ? Color.White * 0.5f : Color.White;

            if (_highlightCounter is not null)
            {
                _highlightCounter -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_highlightCounter <= 0)
                {
                    _highlightCounter = null;
                }
            }
        }

        public IStaticObject StaticObject { get; set; }
        public object Item => StaticObject;
    }
}