﻿using Microsoft.Xna.Framework;

using Zilon.Core.Graphs;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public abstract class GameObjectBase
    {
        public bool CanDraw { get; internal set; }
        public abstract bool HiddenByFow { get; }

        public abstract Vector2 HitEffectPosition { get; }

        public abstract IGraphNode Node { get; }

        public bool UnderFog { get; internal set; }

        public abstract void Draw(GameTime gameTime, Matrix transform);

        public virtual void HandleRemove() { }

        public abstract void Update(GameTime gameTime);
    }
}