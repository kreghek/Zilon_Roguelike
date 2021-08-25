﻿using System;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public static class GameState
    {
        private static int _gameSpeed;

        static GameState()
        {
            GameSpeed = 1;
        }

        public static int GameSpeed
        {
            get => _gameSpeed;
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