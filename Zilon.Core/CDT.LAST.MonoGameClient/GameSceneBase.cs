using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace CDT.LAST.MonoGameClient
{
    /// <summary>
    /// This is the base class for all game scenes.
    /// </summary>
    public abstract class GameSceneBase : DrawableGameComponent
    {
        /// <summary>
        /// List of child GameComponents
        /// </summary>
        private readonly List<GameComponent> components;

        public GameSceneBase(Game game)
            : base(game)
        {
            components = new List<GameComponent>();
            Visible = false;
            Enabled = false;
        }

        /// <summary>
        /// Components of Game Scene
        /// </summary>
        public List<GameComponent> Components => components;

        public GameSceneBase? TargetScene { get; set; }

        /// <summary>
        /// Allows the game component draw your content in game screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Draw the child GameComponents (if drawable)
            for (var i = 0; i < components.Count; i++)
            {
                var gc = components[i];
                if (gc is DrawableGameComponent &&
                    ((DrawableGameComponent)gc).Visible)
                {
                    ((DrawableGameComponent)gc).Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Hide the scene
        /// </summary>
        public virtual void Hide()
        {
            Visible = false;
            Enabled = false;
        }

        /// <summary>
        /// Show the scene
        /// </summary>
        public virtual void Show()
        {
            Visible = true;
            Enabled = true;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Update the child GameComponents
            for (var i = 0; i < components.Count; i++)
            {
                if (components[i].Enabled)
                {
                    components[i].Update(gameTime);
                }
            }

            base.Update(gameTime);
        }
    }
}