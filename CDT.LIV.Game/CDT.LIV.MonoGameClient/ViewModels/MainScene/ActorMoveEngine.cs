using CDT.LIV.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public class ActorMoveEngine
    {
        private double _moveCounter = 1f;

        private readonly Container _rootSprite;

        private Vector2 _targetPosition;
        private Vector2 _startPosition;

        private bool IsComplete => _moveCounter <= 0;



        public ActorMoveEngine(Container rootSprite, Vector2 targetPosition)
        {
            _rootSprite = rootSprite;
            _startPosition = rootSprite.Position;
            _targetPosition = targetPosition;
        }

        public void Update(GameTime gameTime)
        {
            _moveCounter -= gameTime.ElapsedGameTime.TotalSeconds * 0.5;

            _rootSprite.Position = Vector2.Lerp(_startPosition, _targetPosition, (float)_moveCounter);

            if (IsComplete)
            {
                _rootSprite.Position = _targetPosition;
            }
        }
    }
}
