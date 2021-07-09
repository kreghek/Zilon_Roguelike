using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects
{
    /// <summary>
    /// Visual effect of different actions of action aftermaths in game world.
    /// </summary>
    internal interface IVisualEffect
    {
        IEnumerable<GameObjectBase> BoundGameObjects { get; }

        /// <summary>
        /// Determine effect is complete.
        /// Complete effect is not draws youself and can be removed from effect store.
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Draws effect.
        /// </summary>
        /// <param name="spriteBatch"> The sprite batch began in code above. </param>
        void Draw(SpriteBatch spriteBatch, bool backing);

        /// <summary>
        /// Update effect state. Usually it decrements counter of effect's life time and moves effect to complete state.
        /// </summary>
        void Update(GameTime gameTime);
    }
}