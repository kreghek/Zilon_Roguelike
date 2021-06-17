using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public static class GameState
    {
        private static int _gameSpeed;

        public static int GameSpeed
        {
            get
            {
                return _gameSpeed;
            }
            set
            {
                if (value <= 0)
                {
                    throw new InvalidOperationException("Game speed must be more that zero.");
                }

                _gameSpeed = value;
            }
        }
    }
}