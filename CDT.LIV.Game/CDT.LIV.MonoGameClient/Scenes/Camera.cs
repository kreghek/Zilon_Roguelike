using Microsoft.Xna.Framework;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.Scenes
{
    public class Camera
    {
        private const int UNIT_SIZE = 50;

        public Matrix Transform { get; private set; }

        public void Follow(IActorViewModel target, Game game)
        {
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)(target.Actor.Node)).OffsetCoords);

            var position = Matrix.CreateTranslation(
              -playerActorWorldCoords[0] * UNIT_SIZE,
              -playerActorWorldCoords[1] * UNIT_SIZE / 2,
              0);

            var offset = Matrix.CreateTranslation(
                game.GraphicsDevice.Viewport.Width / 2,
                game.GraphicsDevice.Viewport.Height / 2,
                0);

            Transform = position * offset;
        }
    }
}
