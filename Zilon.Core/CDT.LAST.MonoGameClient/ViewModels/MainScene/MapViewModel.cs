using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class MapViewModel
    {
        private const float MAP_UPDATE_DELAY_SECONDS = 0.05f;

        private readonly Texture2D _hexSprite;

        private readonly ConcurrentDictionary<OffsetCoords, Sprite> _hexSprites;
        private readonly IPlayer _player;
        private readonly ISector _sector;
        private readonly SpriteBatch _spriteBatch;
        private readonly ISectorUiState _uiState;

        private double _updateCounter = MAP_UPDATE_DELAY_SECONDS;

        public MapViewModel(Game game, IPlayer player, ISectorUiState uiState, ISector sector, SpriteBatch spriteBatch)
        {
            _hexSprite = game.Content.Load<Texture2D>("Sprites/hex");

            _spriteBatch = spriteBatch;
            _player = player;
            _uiState = uiState;
            _sector = sector;

            _hexSprites = new ConcurrentDictionary<OffsetCoords, Sprite>();
        }

        public void Draw(Matrix transform)
        {
            _spriteBatch.Begin(transformMatrix: transform);

            foreach (var hexSprite in _hexSprites.Values.ToArray())
            {
                hexSprite.Draw(_spriteBatch);
            }

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            UpdateSpriteMatrix(gameTime);
        }

        private void UpdateSpriteMatrix(GameTime gameTime)
        {
            _updateCounter -= gameTime.ElapsedGameTime.TotalSeconds;
            if (_updateCounter > 0)
            {
                return;
            }

            _updateCounter = MAP_UPDATE_DELAY_SECONDS;

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var fowData = _player.MainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(_sector);

            if (visibleFowNodeData is null)
            {
                throw new InvalidOperationException();
            }

            var materializedNodes = visibleFowNodeData.Nodes.ToArray();

            Parallel.ForEach(materializedNodes, fowNode =>
            {
                var node = (HexNode)fowNode.Node;

                Color nodeColor;
                if (_uiState.HoverViewModel != null && node == _uiState.HoverViewModel.Item)
                {
                    nodeColor = Color.CornflowerBlue;
                }
                else
                {
                    nodeColor = Color.White;
                }

                if (_sector.Map.Transitions.ContainsKey(fowNode.Node))
                {
                    nodeColor = Color.Lerp(nodeColor, new Color(255, 0, 0, 255), 0.3f);
                }

                if (fowNode.State != SectorMapNodeFowState.Observing)
                {
                    nodeColor = Color.Lerp(nodeColor, new Color(0, 0, 0, 0), 0.5f);
                }

                if (!_hexSprites.TryGetValue(node.OffsetCoords, out var currentHexSprite))
                {
                    var worldCoords = HexHelper.ConvertToWorld(node.OffsetCoords);
                    var hexSize = MapMetrics.UnitSize / 2;

                    // Remember. Hex width is less that size (radius).
                    // It equals R*Sqrt(3)/2. So sprite width is R*Sqrt(3)/2*2 or R*Sqrt(3). It's about 28 pixels.
                    // You should make sprite 28*16.
                    var newSprite = new Sprite(_hexSprite)
                    {
                        Color = nodeColor,
                        Position = new Vector2(
                            (float)(worldCoords[0] * hexSize * Math.Sqrt(3)),
                            worldCoords[1] * hexSize * 2 / 2
                        )
                    };

                    _hexSprites.AddOrUpdate(node.OffsetCoords, newSprite, (offsetCoords, sprite) => { return sprite; });
                    currentHexSprite = newSprite;
                }

                currentHexSprite.Color = nodeColor;
            });
        }
    }
}