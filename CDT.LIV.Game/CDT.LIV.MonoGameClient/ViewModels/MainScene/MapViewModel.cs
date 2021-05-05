using System;
using System.Threading.Tasks;

using CDT.LIV.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    class MapViewModel : DrawableGameComponent
    {
        private const float MAP_UPDATE_DELAY_SECONDS = 0.05f;
        private double _updateCounter = MAP_UPDATE_DELAY_SECONDS;

        private const int UNIT_SIZE = 32;
        private readonly Texture2D _hexSprite;
        private readonly SpriteBatch _spriteBatch;
        private readonly ISector _sector;
        private readonly IPlayer _player;
        private readonly ISectorUiState _uiState;

        private Sprite[,] _hexSprites = new Sprite[1000, 1000];

        public MapViewModel(Game game, IPlayer player, ISectorUiState uiState, ISector sector, SpriteBatch spriteBatch) : base(game)
        {
            _hexSprite = Game.Content.Load<Texture2D>("Sprites/hex");

            _spriteBatch = spriteBatch;
            _player = player;
            _uiState = uiState;
            _sector = sector;
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

            Parallel.ForEach(visibleFowNodeData.Nodes, (fowNode) =>
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

                var currentHexSprite = _hexSprites[node.OffsetCoords.X, node.OffsetCoords.Y];
                if (currentHexSprite is null)
                {
                    var worldCoords = HexHelper.ConvertToWorld(node.OffsetCoords);
                    var hexSize = UNIT_SIZE / 2;

                    var newSprite = new Sprite(_hexSprite,
                    size: new Point(
                        (int)(hexSize * Math.Sqrt(3)),
                        hexSize * 2 / 2
                        ),
                    color: nodeColor)
                    {
                        Position = new Vector2(
                        (float)(worldCoords[0] * hexSize * Math.Sqrt(3)),
                        (float)(worldCoords[1] * hexSize * 2 / 2)
                        )
                    };

                    _hexSprites[node.OffsetCoords.X, node.OffsetCoords.Y] = newSprite;
                    currentHexSprite = newSprite;
                }

                currentHexSprite.Color = nodeColor;
            });
        }

        public void Update(GameTime gameTime)
        {
            UpdateSpriteMatrix(gameTime);
        }

        public void Draw(Matrix transform)
        {
            _spriteBatch.Begin(transformMatrix: transform);

            for (var x = 0; x < 1000; x++)
            {
                for (var y = 0; y < 1000; y++)
                {
                    var sprite = _hexSprites[x, y];
                    if (sprite != null)
                    {
                        sprite.Draw(_spriteBatch);
                    }
                }
            }

            _spriteBatch.End();
        }
    }
}
