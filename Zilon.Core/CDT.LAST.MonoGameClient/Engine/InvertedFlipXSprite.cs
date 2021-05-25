using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    /// <summary>
    /// Sprite that flip Y if flipped X.
    /// Used to render sprites, rotated on 90/-90 degrees.
    /// </summary>
    public sealed class InvertedFlipXSprite : Sprite
    {
        public InvertedFlipXSprite(Sprite copyFrom, bool includeChildren) : base(copyFrom, includeChildren)
        {
        }

        public InvertedFlipXSprite(Texture2D texture, Point? size = null, Vector2? origin = null,
            Vector2? position = null, Color? color = null, float zindex = 0, Renderable? parent = null) : base(texture,
            size, origin, position, color, zindex, parent)
        {
        }

        protected override SpriteEffects SetFlips(Vector2 scale, ref float rotation)
        {
            // set flips
            var effects = SpriteEffects.None;
            if (scale.X < 0 || scale.Y < 0)
            {
                var rotationVector = EnableRotationFlip
                    ? new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation))
                    : Vector2.One;
                if (scale.X < 0)
                {
                    effects |= SpriteEffects.FlipVertically;
                    rotationVector.X = -rotationVector.X;
                }

                if (scale.Y < 0)
                {
                    effects |= SpriteEffects.FlipHorizontally;
                    rotationVector.Y = -rotationVector.Y;
                }

                // fix rotation
                if (EnableRotationFlip)
                {
                    rotation = (float)Math.Atan2(rotationVector.Y, rotationVector.X);
                }
            }

            return effects;
        }
    }
}