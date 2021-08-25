using System;

using CDT.LAST.MonoGameClient.ViewModels.MainScene;

using Microsoft.Xna.Framework;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.Screens
{
    public class Camera
    {
        private Vector2? _currentPosition;
        private Vector2 _targetPosition;

        public Matrix Transform { get; private set; }

        public void Follow(IActorViewModel target, Game game, GameTime gameTime)
        {
            UpdateTargetPosition(target);

            UpdateTransform(game, gameTime);
        }

        private void UpdateTargetPosition(IActorViewModel target)
        {
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)target.Actor.Node).OffsetCoords);

            var hexSize = MapMetrics.UnitSize / 2;
            var actorPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            _targetPosition = actorPosition;
        }

        private void UpdateTransform(Game game, GameTime gameTime)
        {
            if (_currentPosition is null)
            {
                var position = Matrix.CreateTranslation(
                    -_targetPosition.X,
                    -_targetPosition.Y,
                    0);

                var offset = Matrix.CreateTranslation(
                    (float)game.GraphicsDevice.Viewport.Width / 2,
                    (float)game.GraphicsDevice.Viewport.Height / 2,
                    0);

                Transform = position * offset;

                _currentPosition = _targetPosition;
            }
            else
            {
                if (Vector2.Distance(_currentPosition.Value, _targetPosition) > 0.1)
                {
                    _currentPosition = Vector2.Lerp(_currentPosition.Value, _targetPosition,
                        (float)gameTime.ElapsedGameTime.TotalSeconds * 2f);
                    var gridBoundCurrentPosition =
                        new Vector2((int)Math.Round(_currentPosition.Value.X, MidpointRounding.ToEven),
                            (int)Math.Round(_currentPosition.Value.Y, MidpointRounding.ToEven));

                    var position = Matrix.CreateTranslation(
                        -gridBoundCurrentPosition.X,
                        -gridBoundCurrentPosition.Y,
                        0);

                    var offset = Matrix.CreateTranslation(
                        (float)game.GraphicsDevice.Viewport.Width / 2,
                        (float)game.GraphicsDevice.Viewport.Height / 2,
                        0);

                    Transform = position * offset;
                }
            }
        }
    }
}