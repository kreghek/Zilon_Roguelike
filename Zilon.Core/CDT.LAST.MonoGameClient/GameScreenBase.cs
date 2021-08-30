using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient
{
    /// <summary>
    /// This is the base class for all game scenes.
    /// </summary>
    internal abstract class GameScreenBase : IScreen
    {
        /// <summary>
        /// List of child GameComponents
        /// </summary>
        private readonly IList<GameComponent> _components;

        public GameScreenBase(Game game)
        {
            _components = new List<GameComponent>();

            Game = game;
        }

        protected Game Game { get; }

        public IScreen? TargetScreen { get; set; }

        /// <summary>
        /// Allows the game component draw your content in game screen
        /// </summary>
        public virtual void Draw(GameTime gameTime)
        {
            // Draw the child GameComponents (if drawable)
            for (var i = 0; i < _components.Count; i++)
            {
                var gc = _components[i];

                if (gc is DrawableGameComponent component && component.Visible)
                {
                    component.Draw(gameTime);
                }
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public virtual void Update(GameTime gameTime)
        {
            // Update the child GameComponents
            for (var i = 0; i < _components.Count; i++)
            {
                if (_components[i].Enabled)
                {
                    _components[i].Update(gameTime);
                }
            }
        }
    }
}