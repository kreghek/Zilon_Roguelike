using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    /// <summary>
    /// Structure to pass data into IconButton constructor.
    /// </summary>
    internal record IconData
    {
        public IconData(Texture2D spritesheet, Rectangle sourceRect)
        {
            Spritesheet = spritesheet ?? throw new ArgumentNullException(nameof(spritesheet));
            SourceRect = sourceRect;
        }

        public Texture2D Spritesheet { get; }
        public Rectangle SourceRect { get; }
    }
}