
using Microsoft.Xna.Framework;

namespace CDT.LIV.MonoGameClient.Engine
{
    /// <summary>
    /// Renderable transformations - position, scale, rotation, color, etc.
    /// </summary>
    public class SpriteTransformation
    {
        // identity transformation object.

        /// <summary>
        /// Drawing color (work as tint color).
        /// </summary>
        public Color Color = Color.White;

        /// <summary>
        /// Transformation position.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// Transformation rotation.
        /// </summary>
        public float Rotation;

        /// <summary>
        /// Transformation scale.
        /// </summary>
        public Vector2 Scale = Vector2.One;

        /// <summary>
        /// Get the identity transformations.
        /// </summary>
        public static SpriteTransformation Identity { get; } = new SpriteTransformation();

        /// <summary>
        /// Clone the transformations.
        /// </summary>
        /// <returns>Cloned transformations.</returns>
        public SpriteTransformation Clone()
        {
            SpriteTransformation ret = new SpriteTransformation();
            ret.Color = Color;
            ret.Rotation = Rotation;
            ret.Scale = Scale;
            ret.Position = Position;
            return ret;
        }

        /// <summary>
        /// Merge two transformations into one.
        /// </summary>
        /// <param name="a">First transformation.</param>
        /// <param name="b">Second transformation.</param>
        /// <returns>Merged transformation.</returns>
        public static SpriteTransformation Compose(SpriteTransformation a, SpriteTransformation b)
        {
            SpriteTransformation result = new SpriteTransformation();
            Vector2 transformedPosition = a.TransformVector(b.Position);
            result.Position = transformedPosition;
            result.Rotation = a.Rotation + b.Rotation;
            result.Scale = a.Scale * b.Scale;
            result.Color = MultiplyColors(a.Color, b.Color);
            return result;
        }

        /// <summary>
        /// Lerp between two transformation states.
        /// </summary>
        /// <param name="key1">Transformations from.</param>
        /// <param name="key2">Transformations to.</param>
        /// <param name="amount">How much to lerp.</param>
        /// <param name="result">Out transformations.</param>
        public static void Lerp(ref SpriteTransformation key1, ref SpriteTransformation key2, float amount,
            ref SpriteTransformation result)
        {
            result.Position = Vector2.Lerp(key1.Position, key2.Position, amount);
            result.Scale = Vector2.Lerp(key1.Scale, key2.Scale, amount);
            result.Rotation = MathHelper.Lerp(key1.Rotation, key2.Rotation, amount);
        }

        /// <summary>
        /// Multiply two colors and return the result.
        /// </summary>
        /// <param name="a">Color a to multiply.</param>
        /// <param name="b">Color b to multiply.</param>
        /// <returns>Result color.</returns>
        public static Color MultiplyColors(Color a, Color b)
        {
            return new Color(
                ((float)a.R / 255f) * ((float)b.R / 255f),
                ((float)a.G / 255f) * ((float)b.G / 255f),
                ((float)a.B / 255f) * ((float)b.B / 255f),
                ((float)a.A / 255f) * ((float)b.A / 255f));
        }

        /// <summary>
        /// Transform a vector.
        /// </summary>
        /// <param name="point">Vector to transform.</param>
        /// <returns>Transformed vector.</returns>
        public Vector2 TransformVector(Vector2 point)
        {
            Vector2 result = Vector2.Transform(point, Matrix.CreateRotationZ(Rotation * System.Math.Sign(Scale.X)));
            result *= Scale;
            result += Position;
            return result;
        }
    }
}