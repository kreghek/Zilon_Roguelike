using Microsoft.Xna.Framework;

using Zilon.Core.Graphs;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public abstract class GameObjectBase
    {
        public abstract bool HiddenByFow { get; }

        public abstract Vector2 HitEffectPosition { get; }

        public abstract IGraphNode Node { get; }
        public bool Visible { get; internal set; }

        public abstract void Draw(GameTime gameTime, Matrix transform);

        public abstract void Update(GameTime gameTime);
    }
}