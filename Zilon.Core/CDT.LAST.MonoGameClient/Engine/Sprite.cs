using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LIV.MonoGameClient.Engine
{
    /// <summary>
    /// Renderable transformations - position, scale, rotation, color, etc.
    /// </summary>
    public class Transformation
    {
        /// <summary>
        /// Transformation position.
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// Transformation scale.
        /// </summary>
        public Vector2 Scale = Vector2.One;

        /// <summary>
        /// Transformation rotation.
        /// </summary>
        public float Rotation = 0;

        /// <summary>
        /// Drawing color (work as tint color).
        /// </summary>
        public Color Color = Color.White;

        // identity transformation object.
        private static readonly Transformation _identity = new Transformation();

        /// <summary>
        /// Get the identity transformations.
        /// </summary>
        public static Transformation Identity { get { return _identity; } }

        /// <summary>
        /// Clone the transformations.
        /// </summary>
        /// <returns>Cloned transformations.</returns>
        public Transformation Clone()
        {
            Transformation ret = new Transformation();
            ret.Color = Color;
            ret.Rotation = Rotation;
            ret.Scale = Scale;
            ret.Position = Position;
            return ret;
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
        /// Merge two transformations into one.
        /// </summary>
        /// <param name="a">First transformation.</param>
        /// <param name="b">Second transformation.</param>
        /// <returns>Merged transformation.</returns>
        public static Transformation Compose(Transformation a, Transformation b)
        {
            Transformation result = new Transformation();
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
        public static void Lerp(ref Transformation key1, ref Transformation key2, float amount, ref Transformation result)
        {
            result.Position = Vector2.Lerp(key1.Position, key2.Position, amount);
            result.Scale = Vector2.Lerp(key1.Scale, key2.Scale, amount);
            result.Rotation = MathHelper.Lerp(key1.Rotation, key2.Rotation, amount);
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

    /// <summary>
    /// A renderable entity.
    /// </summary>
    public class Renderable
    {
        /// <summary>
        /// Parent entity.
        /// </summary>
        protected Renderable? _parent { get; set; } = null;

        /// <summary>
        /// Local transformations (color, position, rotation..).
        /// </summary>
        protected Transformation _localTrans = new Transformation();

        /// <summary>
        /// World transformations (local transformations + parent's world transformations).
        /// These are the actual transformations that will apply when drawing the entity.
        /// </summary>
        private Transformation _worldTrans = new Transformation();

        /// <summary>
        /// Get final world transformations.
        /// </summary>
        public Transformation WorldTransformations { get { return _worldTrans; } }

        /// <summary>
        /// Is the entity currently visible?
        /// </summary>
        public bool Visible { get; set; } = true;

        // currently calculated z-index, including parents.
        private float _finalZindex = 0f;

        // do we need to update transformations?
        private bool _needUpdateTransformations = false;

        /// <summary>
        /// Child entities.
        /// </summary>
        protected List<Renderable> _children = new List<Renderable>();

        /// <summary>
        /// String identifier you can attach to renderable entities.
        /// </summary>
        public string? Identifier { get; set; }

        /// <summary>
        /// Should we flip drawing on X axis?
        /// </summary>
        public bool FlipX
        {
            set
            {
                _localTrans.Scale.X = System.Math.Abs(_localTrans.Scale.X) * (value ? -1f : 1f);
                UpdateTransformations();
            }
            get
            {
                return _localTrans.Scale.X < 0f;
            }
        }

        /// <summary>
        /// Should we flip drawing on Y axis?
        /// </summary>
        public bool FlipY
        {
            set
            {
                _localTrans.Scale.Y = System.Math.Abs(_localTrans.Scale.Y) * (value ? -1f : 1f);
                UpdateTransformations();
            }
            get
            {
                return _localTrans.Scale.Y < 0f;
            }
        }

        /// <summary>
        /// Renderable position.
        /// </summary>
        public Vector2 Position { get { return _localTrans.Position; } set { _localTrans.Position = value; UpdateTransformations(); } }

        /// <summary>
        /// Renderable scale.
        /// </summary>
        public Vector2 Scale { get { return _localTrans.Scale; } set { _localTrans.Scale = value; UpdateTransformations(); } }

        /// <summary>
        /// Renderable scale as a single scalar (in oppose to a vector).
        /// </summary>
        public float ScaleScalar { get { return _localTrans.Scale.X; } set { Scale = Vector2.One * value; } }

        /// <summary>
        /// Renderable rotation (radians).
        /// </summary>
        public float Rotation { get { return _localTrans.Rotation; } set { _localTrans.Rotation = value; UpdateTransformations(); } }

        /// <summary>
        /// Renderable z-index (relative to parent).
        /// </summary>
        public float Zindex { get { return _zindex; } set { _zindex = value; UpdateTransformations(); } }
        private float _zindex = 0f;

        /// <summary>
        /// Renderable tint color.
        /// </summary>
        public Color Color { get { return _localTrans.Color; } set { _localTrans.Color = value; UpdateTransformations(); } }

        /// <summary>
        /// If true, will normalize Z-index to always be between 0-1 values (note: this divide the final z-index by max float).
        /// </summary>
        public bool NormalizeZindex = false;

        /// <summary>
        /// Create the new renderable entity with default values.
        /// </summary>
        public Renderable()
        {
        }

        /// <summary>
        /// Called whenever one of the transformations properties change and we need to update world transformations.
        /// </summary>
        protected void UpdateTransformations()
        {
            _needUpdateTransformations = true;
        }

        /// <summary>
        /// Add a child entity to this renderable.
        /// </summary>
        /// <param name="child">Child entity to add.</param>
        public void AddChild(Renderable child)
        {
            // if child already got a parent throw exception
            if (child._parent != null)
            {
                throw new System.Exception("Renderable to add as child already have a parent!");
            }

            // add child
            _children.Add(child);
            child._parent = this;

            // update child transformations (since now it got a new parent)
            child.UpdateTransformations();
        }

        /// <summary>
        /// Get child by index.
        /// </summary>
        /// <param name="index">Child index to get.</param>
        /// <returns>Return child.</returns>
        public Renderable GetChild(int index)
        {
            return _children[index];
        }

        /// <summary>
        /// Remove a child entity from this renderable.
        /// </summary>
        /// <param name="child">Child entity to remove.</param>
        public void RemoveChild(Renderable child)
        {
            // if child don't belong to this entity throw exception
            if (child._parent != this)
            {
                throw new System.Exception("Renderable to remove is not a child of this renderable!");
            }

            // remove child
            _children.Remove(child);
            child._parent = null;

            // update child transformations (since now it no longer got a parent)
            child.UpdateTransformations();
        }

        /// <summary>
        /// Draw this renderable entity.
        /// This will also update transformations if needed.
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to use for drawing.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // not visible? skip!
            if (!Visible)
            {
                return;
            }

            // if need to update transformations:
            if (_needUpdateTransformations)
            {
                // create world transformations (merged with parent)
                _worldTrans = _parent != null ? Transformation.Compose(_parent._worldTrans, _localTrans) : _localTrans;

                // calculate final zindex
                _finalZindex = (_parent != null ? _parent._finalZindex + Zindex : Zindex);

                // notify all childrens that they also need update
                foreach (var child in _children)
                {
                    child.UpdateTransformations();
                }

                // no longer need to update transformations
                _needUpdateTransformations = false;
            }

            // draw the entity
            DoDraw(spriteBatch, _finalZindex);

            // draw children
            foreach (var child in _children)
            {
                child.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Do the object-specific drawing function.
        /// Implement per renderable type.
        /// </summary>
        /// <remarks>When this function is called, transformation is already applied (position / scale / rotation).</remarks>
        /// <param name="spriteBatch">Spritebatch to use for drawing.</param>
        /// <param name="zindex">Final rendering zindex.</param>
        protected virtual void DoDraw(SpriteBatch spriteBatch, float zindex)
        {
        }

        /// <summary>
        /// Clone this renderable object.
        /// </summary>
        /// <param name="includeChildren">If true, will include children in clone.</param>
        /// <returns>Cloned object.</returns>
        virtual public Renderable Clone(bool includeChildren)
        {
            return new Renderable(this, includeChildren);
        }

        /// <summary>
        /// Clone an existing renderable object.
        /// </summary>
        /// <param name="copyFrom">Object to copy properties from.</param>
        /// <param name="includeChildren">If true, will also clone children.</param>
        public Renderable(Renderable copyFrom, bool includeChildren)
        {
            // copy basics
            Visible = copyFrom.Visible;
            Zindex = copyFrom.Zindex;
            _localTrans = copyFrom._localTrans.Clone();

            // clone children
            if (includeChildren)
            {
                foreach (var child in copyFrom._children)
                {
                    AddChild(child.Clone(true));
                }
            }

            // update transformations
            UpdateTransformations();
        }
    }

    /// <summary>
    /// Basic sprite entity (renderable image).
    /// https://github.com/RonenNess/MonoGame-Sprites/blob/master/MonoSprites/Source/Sprite.cs
    /// </summary>
    public class Sprite : Renderable
    {
        /// <summary>
        /// Sprite origin / source, eg pivot point for rotation etc.
        /// </summary>
        public Vector2 Origin = Vector2.One * 0.5f;

        /// <summary>
        /// Texture to draw.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Size, in pixels, we want this sprite to be when rendered.
        /// </summary>
        public Point Size { get; set; }

        /// <summary>
        /// Optional texture source rectangle.
        /// </summary>
        public Rectangle? SourceRectangle;

        /// <summary>
        /// If true, will also flip rotation on X and Y axis when there's a flip.
        /// </summary>
        public bool EnableRotationFlip = false;

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
        public Sprite(Texture2D texture, Point? size = null, Vector2? origin = null, Vector2? position = null, Color? color = null, float zindex = 0f, Renderable? parent = null) : base()
        {
            Size = size ?? Point.Zero;
            Texture = texture;
            Origin = origin ?? Vector2.One * 0.5f;
            Position = position ?? Vector2.Zero;
            Color = color ?? Color.White;
            Zindex = zindex;
            if (parent != null)
            { parent.AddChild(this); }
        }

        /// <summary>
        /// Clone this sprite object.
        /// </summary>
        /// <param name="includeChildren">If true, will include children in clone.</param>
        /// <returns>Cloned object.</returns>
        override public Renderable Clone(bool includeChildren)
        {
            return new Sprite(this, includeChildren);
        }

        /// <summary>
        /// Clone an existing Sprite object.
        /// </summary>
        /// <param name="copyFrom">Sprite to copy properties from.</param>
        /// <param name="includeChildren">If true, will also clone children.</param>
        public Sprite(Sprite copyFrom, bool includeChildren) : base(copyFrom, includeChildren)
        {
            SourceRectangle = copyFrom.SourceRectangle;
            Origin = copyFrom.Origin;
            Texture = copyFrom.Texture;
            Size = copyFrom.Size;
        }

        /// <summary>
        /// Set a source rectangle from spritesheet.
        /// </summary>
        /// <param name="index">Sprite index to pick.</param>
        /// <param name="spritesCount">Number of sprites on X and Y axis.</param>
        public void SetSourceFromSpritesheet(Point index, Point spritesCount)
        {
            Point size = Texture.Bounds.Size / spritesCount;
            SourceRectangle = new Rectangle(index * size, size);
        }

        /// <summary>
        /// Set a source rectangle from spritesheet.
        /// </summary>
        /// <param name="index">Sprite index to pick.</param>
        /// <param name="spritesCount">Number of sprites on X and Y axis.</param>
        /// <param name="rectSize">Size of the rectangle to set based on number of sprites in sheet.</param>
        public void SetSourceFromSpritesheet(Point index, Point spritesCount, Point rectSize)
        {
            Point size = Texture.Bounds.Size / spritesCount;
            SourceRectangle = new Rectangle(index * size, size * rectSize);
        }

        /// <summary>
        /// Draw the sprite itself.
        /// </summary>
        /// <remarks>When this function is called, transformation is already applied (position / scale / rotation).</remarks>
        /// <param name="spriteBatch">Spritebatch to use for drawing.</param>
        /// <param name="zindex">Final rendering zindex.</param>
        protected override void DoDraw(SpriteBatch spriteBatch, float zindex)
        {
            // no texture? skip
            if (Texture == null)
            {
                return;
            }

            // if source rect is 0,0, set to texture default size
            var _srcRect = SourceRectangle ?? new Rectangle(0, 0, 0, 0);
            if (_srcRect.Width == 0)
            { _srcRect.Width = Texture.Width; }
            if (_srcRect.Height == 0)
            { _srcRect.Height = Texture.Height; }

            // calculate origin point
            Vector2 origin = new Vector2(_srcRect.Width * Origin.X, _srcRect.Height * Origin.Y);

            // get scale from transformations
            Vector2 scale = WorldTransformations.Scale;

            // take desired size into consideration
            if (Size.X != 0)
            {
                scale.X *= (float)Size.X / Texture.Width;
                scale.Y *= (float)Size.Y / Texture.Height;
            }

            // get rotation
            var rotation = WorldTransformations.Rotation;

            // set flips
            var effects = SpriteEffects.None;
            if (scale.X < 0 || scale.Y < 0)
            {
                var rotationVector = EnableRotationFlip ? new Vector2((float)System.Math.Cos(rotation), (float)System.Math.Sin(rotation)) : Vector2.One;
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
                    rotation = (float)System.Math.Atan2(rotationVector.Y, rotationVector.X);
                }
            }

            // normalize z-index
            if (NormalizeZindex)
            {
                if (zindex < 0f)
                    zindex = 0f;
                zindex /= float.MaxValue;
            }

            // draw the sprite
            spriteBatch.Draw(
                texture: Texture,
                position: WorldTransformations.Position,
                sourceRectangle: _srcRect,
                color: WorldTransformations.Color,
                rotation: rotation,
                origin: origin,
                scale: new Vector2(System.Math.Abs(scale.X), System.Math.Abs(scale.Y)),
                effects: effects,
                layerDepth: zindex);
        }
    }


    /// <summary>
    /// A container that can hold and draw internal entities with shared transformations.
    /// </summary>
    public class Container : Renderable
    {
        /// <summary>
        /// Create the new container.
        /// </summary>
        public Container() : base()
        {
        }
    }
}