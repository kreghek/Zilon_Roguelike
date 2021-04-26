using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Common;
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
        private readonly ISectorUiState _uiState;

        public MapViewModel(Game game, ISectorUiState uiState, ISector sector, SpriteBatch spriteBatch) : base(game)
        {
            _hexSprite = Game.Content.Load<Texture2D>("Sprites/hex");

            _spriteBatch = spriteBatch;
            _uiState = uiState;
            _sector = sector;
        }

        public void Draw()
        {
            _spriteBatch.Begin();
            foreach (HexNode node in _sector.Map.Nodes)
            {
                var worldCoords = HexHelper.ConvertToWorld(node.OffsetCoords);

                if (_uiState.HoverViewModel != null && node == _uiState.HoverViewModel.Item)
                {
                    _spriteBatch.Draw(_hexSprite, new Rectangle((int)(worldCoords[0] * UNIT_SIZE), (int)(worldCoords[1] * UNIT_SIZE / 2), UNIT_SIZE, UNIT_SIZE / 2), Color.CornflowerBlue);
                }
                else
                {
                    _spriteBatch.Draw(_hexSprite, new Rectangle((int)(worldCoords[0] * UNIT_SIZE), (int)(worldCoords[1] * UNIT_SIZE / 2), UNIT_SIZE, UNIT_SIZE / 2), Color.White);
                }
            }
            _spriteBatch.End();
        }
    }
}
