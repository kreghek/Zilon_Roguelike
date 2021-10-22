using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    /// <summary>
    /// Sprite that flip Y if flipped X.
    /// Used to render sprites, rotated on 90/-90 degrees.
    /// </summary>
    internal sealed class InvertedFlipXSprite : Sprite
    {
        public InvertedFlipXSprite(Texture2D texture) : base(texture)
        {
        }

        protected override FlipResult SetFlips(Vector2 scale, float rotation)
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

            return new FlipResult { Effects = effects, Rotation = rotation };
        }
    }
}