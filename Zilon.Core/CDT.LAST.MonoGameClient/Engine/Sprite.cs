using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    /// <summary>
    /// Basic sprite entity (renderable image).
    /// https://github.com/RonenNess/MonoGame-Sprites/blob/master/MonoSprites/Source/Sprite.cs
    /// </summary>
    internal class Sprite : Renderable
    {
        /// <summary>
        /// Size, in pixels, we want this sprite to be when rendered.
        /// </summary>
        private readonly Point _size;

        /// <summary>
        /// Texture to draw.
        /// </summary>
        private readonly Texture2D _texture;

        /// <summary>
        /// If true, will also flip rotation on X and Y axis when there's a flip.
        /// </summary>
        protected bool EnableRotationFlip = false;

        /// <summary>
        /// Sprite origin / source, eg pivot point for rotation etc.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// Optional texture source rectangle.
        /// </summary>
        public Rectangle? SourceRectangle;

        /// <summary>
        /// Create the new sprite entity with params.
        /// </summary>
        /// <param name="texture">Texture to use for this sprite.</param>
        /// <param name="size">Sprite starting size.</param>
        /// <param name="origin">Origin of the sprite (also known as anchor point) relative to drawing size.</param>
        /// <param name="position">Sprite local position.</param>
        /// <param name="color">Sprite color.</param>
        /// <param name="zindex">Sprite zindex.</param>
        /// <param name="parent">Parent container.</param>
        public Sprite(Texture2D texture, Point? size = null, Vector2? origin = null, Vector2? position = null,
            Color? color = null, float zindex = 0f, Renderable? parent = null)
        {
            _size = size ?? Point.Zero;
            _texture = texture;
            Origin = origin ?? Vector2.One * 0.5f;
            Position = position ?? Vector2.Zero;
            Color = color ?? Color.White;
            Zindex = zindex;
            if (parent != null)
            {
                parent.AddChild(this);
            }
        }

        /// <summary>
        /// Draw the sprite itself.
        /// </summary>
        /// <remarks>When this function is called, transformation is already applied (position / scale / rotation).</remarks>
        /// <param name="spriteBatch">Spritebatch to use for drawing.</param>
        /// <param name="zindex">Final rendering zindex.</param>
        protected override void DoDraw(SpriteBatch spriteBatch, float zindex)
        {
            // if source rect is 0,0, set to texture default size
            var srcRect = SourceRectangle ?? new Rectangle(0, 0, 0, 0);
            if (srcRect.Width == 0)
            {
                srcRect.Width = _texture.Width;
            }

            if (srcRect.Height == 0)
            {
                srcRect.Height = _texture.Height;
            }

            // calculate origin point
            var origin = new Vector2(srcRect.Width * Origin.X, srcRect.Height * Origin.Y);

            // get scale from transformations
            var scale = WorldTransformations.Scale;

            // take desired size into consideration
            if (_size.X != 0)
            {
                scale.X *= (float)_size.X / _texture.Width;
                scale.Y *= (float)_size.Y / _texture.Height;
            }

            // get rotation
            var rotation = WorldTransformations.Rotation;

            var flipResult = SetFlips(scale, rotation);

            // normalize z-index
            if (NormalizeZindex)
            {
                if (zindex < 0f)
                {
                    zindex = 0f;
                }

                zindex /= float.MaxValue;
            }

            // draw the sprite
            spriteBatch.Draw(
                texture: _texture,
                position: WorldTransformations.Position,
                sourceRectangle: srcRect,
                color: WorldTransformations.Color,
                rotation: flipResult.Rotation,
                origin: origin,
                scale: new Vector2(Math.Abs(scale.X), Math.Abs(scale.Y)),
                effects: flipResult.Effects,
                layerDepth: zindex);
        }

        protected virtual FlipResult SetFlips(Vector2 scale, float rotation)
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
                    effects |= SpriteEffects.FlipHorizontally;
                    rotationVector.X = -rotationVector.X;
                }

                if (scale.Y < 0)
                {
                    effects |= SpriteEffects.FlipVertically;
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

        protected record FlipResult
        {
            public SpriteEffects Effects { get; init; }
            public float Rotation { get; init; }
        }
    }
}