﻿using Microsoft.Xna.Framework;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.Scenes
{
    public class Camera
    {
        private const int UNIT_SIZE = 32;

        public Matrix Transform { get; private set; }

        public void Follow(IActorViewModel target, Game game)
        {
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)target.Actor.Node).OffsetCoords);

            var hexSize = UNIT_SIZE / 2;
            var actorPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * System.Math.Sqrt(3)),
                (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
            );

            var position = Matrix.CreateTranslation(
                -actorPosition.X,
                -actorPosition.Y,
                0);

            var offset = Matrix.CreateTranslation(
                game.GraphicsDevice.Viewport.Width / 2,
                game.GraphicsDevice.Viewport.Height / 2,
                0);

            Transform = position * offset;
        }
    }
}