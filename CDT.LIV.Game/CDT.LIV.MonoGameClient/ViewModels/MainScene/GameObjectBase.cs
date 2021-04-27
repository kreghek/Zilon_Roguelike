using Microsoft.Xna.Framework;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public abstract class GameObjectBase
    {
        public abstract void Draw(GameTime gameTime, Matrix transform);

        public abstract void Update(GameTime gameTime);
    }
}
