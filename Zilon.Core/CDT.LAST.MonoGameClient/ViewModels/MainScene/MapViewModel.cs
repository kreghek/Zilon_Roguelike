using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
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
        private readonly Game _game;

        private readonly Texture2D _hexSprite;
        private readonly Texture2D _hexMarkerTextures;
        private readonly ConcurrentDictionary<OffsetCoords, SpriteContainer> _hexSprites;
        private readonly IPlayer _player;
        private readonly ISector _sector;
        private readonly SpriteBatch _spriteBatch;
        private readonly ISectorUiState _uiState;

        private double _updateCounter = MAP_UPDATE_DELAY_SECONDS;

        public MapViewModel(Game game, IPlayer player, ISectorUiState uiState, ISector sector, SpriteBatch spriteBatch)
        {
            _hexSprite = game.Content.Load<Texture2D>("Sprites/hex");
            _hexMarkerTextures = game.Content.Load<Texture2D>("Sprites/ui/HexMarkers");

            _spriteBatch = spriteBatch;
            _game = game;
            _player = player;
            _uiState = uiState;
            _sector = sector;

            _hexSprites = new ConcurrentDictionary<OffsetCoords, SpriteContainer>();

            sector.TrasitionUsed += Sector_TrasitionUsed;
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
            try
            {
                UpdateSpriteMatrix(gameTime);
            }
            catch (ArgumentException)
            {
                // Do nothing.
                // visibleFowNodeData.Nodes can throw the exception in multuthreading environment.
                // This can crash the game.
                // But we can ignore this frame and try to make the new one. User will not see the broken frame.
            }
        }

        private void Sector_TrasitionUsed(object? sender, TransitionUsedEventArgs e)
        {
            if (e.Actor.Person == _player.MainPerson)
            {
                var blockerService = ((LivGame)_game).ServiceProvider.GetRequiredService<IAnimationBlockerService>();
                blockerService.DropBlockers();
                _sector.TrasitionUsed -= Sector_TrasitionUsed;
            }
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

                if (fowNode.State != SectorMapNodeFowState.Observing)
                {
                    nodeColor = Color.Lerp(nodeColor, new Color(0, 0, 0, 0), 0.5f);
                }

                if (!_hexSprites.TryGetValue(node.OffsetCoords, out var currentHexSprite))
                {
                    var worldCoords = HexHelper.ConvertToWorld(node.OffsetCoords);
                    var hexSize = MapMetrics.UnitSize / 2;

                    var hexTextureIndex = node.GetHashCode() % 4;
                    var hexTextureIndexX = hexTextureIndex / 2;
                    var hexTextureIndexY = hexTextureIndex % 2;

                    // Remember. Hex width is less that size (radius).
                    // It equals R*Sqrt(3)/2. So sprite width is R*Sqrt(3)/2*2 or R*Sqrt(3). It's about 28 pixels.
                    // You should make sprite 28*16.
                    var hexSprite = new Sprite(_hexSprite)
                    {
                        Color = nodeColor,
                        SourceRectangle = new Rectangle(hexTextureIndexX * 28, hexTextureIndexY * 16, 28, 16)
                    };

                    var hexSpriteContainer = new SpriteContainer {
                        Position = new Vector2(
                            (float)(worldCoords[0] * hexSize * Math.Sqrt(3)),
                            worldCoords[1] * hexSize * 2 / 2
                        ),
                    };
                    hexSpriteContainer.AddChild(hexSprite);

                    if (_sector.Map.Transitions.TryGetValue(fowNode.Node, out var transition))
                    {
                        if (transition.SectorNode.SectorScheme.Sid == "dungeon" || transition.SectorNode.SectorScheme.Sid == "elder-temple")
                        {
                            var transitionMarkerSprite = new Sprite(_hexMarkerTextures)
                            {
                                SourceRectangle = new Rectangle(28, 0, 28, 16)
                            };
                            hexSpriteContainer.AddChild(transitionMarkerSprite);
                        }
                        else
                        {
                            var transitionMarkerSprite = new Sprite(_hexMarkerTextures)
                            {
                                SourceRectangle = new Rectangle(0, 0, 28, 16)
                            };
                            hexSpriteContainer.AddChild(transitionMarkerSprite);
                        }
                    }

                    _hexSprites.AddOrUpdate(node.OffsetCoords, hexSpriteContainer, (offsetCoords, sprite) => { return sprite; });
                    currentHexSprite = hexSpriteContainer;
                }

                currentHexSprite.Color = nodeColor;
            });
        }
    }
}