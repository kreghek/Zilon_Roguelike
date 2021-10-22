using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Engine
{
    /// <summary>
    /// A renderable entity.
    /// </summary>
    internal abstract class Renderable
    {
        /// <summary>
        /// Child entities.
        /// </summary>
        protected readonly IList<Renderable> Children;

        // currently calculated z-index, including parents.
        private float _finalZindex;

        /// <summary>
        /// Local transformations (color, position, rotation..).
        /// </summary>
        private readonly SpriteTransformation _localTrans;

        // do we need to update transformations?
        private bool _needUpdateTransformations;

        /// <summary>
        /// World transformations (local transformations + parent's world transformations).
        /// These are the actual transformations that will apply when drawing the entity.
        /// </summary>
        private SpriteTransformation _worldTrans;

        private float _zindex;

        /// <summary>
        /// If true, will normalize Z-index to always be between 0-1 values (note: this divide the final z-index by max float).
        /// </summary>
        protected bool NormalizeZindex = false;

        /// <summary>
        /// Create the new renderable entity with default values.
        /// </summary>
        public Renderable()
        {
            Children = new List<Renderable>();
            _localTrans = new SpriteTransformation();
            _worldTrans = new SpriteTransformation();
        }

        /// <summary>
        /// Renderable tint color.
        /// </summary>
        public Color Color
        {
            get => _localTrans.Color;
            set
            {
                _localTrans.Color = value;
                UpdateTransformations();
            }
        }

        /// <summary>
        /// Should we flip drawing on X axis?
        /// </summary>
        public bool FlipX
        {
            set
            {
                _localTrans.Scale.X = Math.Abs(_localTrans.Scale.X) * (value ? -1f : 1f);
                UpdateTransformations();
            }
            get => _localTrans.Scale.X < 0f;
        }

        /// <summary>
        /// Should we flip drawing on Y axis?
        /// </summary>
        public bool FlipY
        {
            set
            {
                _localTrans.Scale.Y = Math.Abs(_localTrans.Scale.Y) * (value ? -1f : 1f);
                UpdateTransformations();
            }
            get => _localTrans.Scale.Y < 0f;
        }

        /// <summary>
        /// String identifier you can attach to renderable entities.
        /// </summary>
        public string? Identifier { get; set; }

        /// <summary>
        /// Renderable position.
        /// </summary>
        public Vector2 Position
        {
            get => _localTrans.Position;
            set
            {
                _localTrans.Position = value;
                UpdateTransformations();
            }
        }

        /// <summary>
        /// Renderable rotation (radians).
        /// </summary>
        public float Rotation
        {
            get => _localTrans.Rotation;
            set
            {
                _localTrans.Rotation = value;
                UpdateTransformations();
            }
        }

        /// <summary>
        /// Renderable scale.
        /// </summary>
        public Vector2 Scale
        {
            get => _localTrans.Scale;
            set
            {
                _localTrans.Scale = value;
                UpdateTransformations();
            }
        }

        /// <summary>
        /// Renderable scale as a single scalar (in oppose to a vector).
        /// </summary>
        public float ScaleScalar
        {
            get => _localTrans.Scale.X;
            set => Scale = Vector2.One * value;
        }

        /// <summary>
        /// Is the entity currently visible?
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Get final world transformations.
        /// </summary>
        public SpriteTransformation WorldTransformations => _worldTrans;

        /// <summary>
        /// Renderable z-index (relative to parent).
        /// </summary>
        public float Zindex
        {
            get => _zindex;
            set
            {
                _zindex = value;
                UpdateTransformations();
            }
        }

        /// <summary>
        /// Parent entity.
        /// </summary>
        protected Renderable? Parent { get; set; }

        /// <summary>
        /// Add a child entity to this renderable.
        /// </summary>
        /// <param name="child">Child entity to add.</param>
        public void AddChild(Renderable child)
        {
            // if child already got a parent throw exception
            if (child.Parent != null)
            {
                throw new Exception("Renderable to add as child already have a parent!");
            }

            // add child
            Children.Add(child);
            child.Parent = this;

            // update child transformations (since now it got a new parent)
            child.UpdateTransformations();
            AfterAddChild(child);
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
                _worldTrans = Parent != null
                    ? SpriteTransformation.Compose(Parent._worldTrans, _localTrans)
                    : _localTrans;

                // calculate final zindex
                _finalZindex = Parent != null ? Parent._finalZindex + Zindex : Zindex;

                // notify all children that they also need update
                foreach (var child in Children)
                {
                    child.UpdateTransformations();
                }

                // no longer need to update transformations
                _needUpdateTransformations = false;
            }

            // draw the entity
            DoDraw(spriteBatch, _finalZindex);

            // draw children
            foreach (var child in Children)
            {
                child.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Returns all children to enumerate.
        /// </summary>
        /// <returns> Set of all children of the renderable object. </returns>
        public IEnumerable<Renderable> GetChildren()
        {
            return Children;
        }

        /// <summary>
        /// Remove a child entity from this renderable.
        /// </summary>
        /// <param name="child">Child entity to remove.</param>
        public void RemoveChild(Renderable child)
        {
            // if child don't belong to this entity throw exception
            if (child.Parent != this)
            {
                throw new Exception("Renderable to remove is not a child of this renderable!");
            }

            // remove child
            Children.Remove(child);
            child.Parent = null;

            // update child transformations (since now it no longer got a parent)
            child.UpdateTransformations();
        }

        protected virtual void AfterAddChild(Renderable child) { }

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
        /// Called whenever one of the transformations properties change and we need to update world transformations.
        /// </summary>
        private void UpdateTransformations()
        {
            _needUpdateTransformations = true;
        }
    }
}