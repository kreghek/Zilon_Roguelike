using System;

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
        private const int UNIT_SIZE = 50;
        private readonly Texture2D _hexSprite;
        private readonly SpriteBatch _spriteBatch;
        private readonly ISector _sector;
        private readonly IPlayer _player;
        private readonly ISectorUiState _uiState;

        public MapViewModel(Game game, IPlayer player, ISectorUiState uiState, ISector sector, SpriteBatch spriteBatch) : base(game)
        {
            _hexSprite = Game.Content.Load<Texture2D>("Sprites/hex");

            _spriteBatch = spriteBatch;
            _player = player;
            _uiState = uiState;
            _sector = sector;
        }

        public void Draw(Matrix transform)
        {
            _spriteBatch.Begin(transformMatrix: transform);

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

            foreach (var fowNode in visibleFowNodeData.Nodes)
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

                var worldCoords = HexHelper.ConvertToWorld(node.OffsetCoords);
                var hexSize = UNIT_SIZE / 2;
                var sprite = new Sprite(_hexSprite,
                    size: new Point(
                        (int)(hexSize * System.Math.Sqrt(3)),
                        hexSize * 2 / 2
                        ),
                    color: nodeColor);

                sprite.Position = new Vector2(
                    (float)(worldCoords[0] * hexSize * System.Math.Sqrt(3)),
                    (float)(worldCoords[1] * hexSize * 2 / 2)
                    );

                sprite.Draw(_spriteBatch);

            }
            _spriteBatch.End();
        }
    }
}
