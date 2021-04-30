using System;

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
                _spriteBatch.Draw(
                    _hexSprite,
                    new Rectangle(
                        (int)(worldCoords[0] * UNIT_SIZE - UNIT_SIZE / 2),
                        (int)(worldCoords[1] * UNIT_SIZE / 2 - UNIT_SIZE / 4),
                        UNIT_SIZE,
                        UNIT_SIZE / 2),
                    nodeColor);
            }
            _spriteBatch.End();
        }
    }
}
