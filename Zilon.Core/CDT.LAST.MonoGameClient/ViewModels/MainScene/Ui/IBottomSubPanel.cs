using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public interface IBottomSubPanel
    {
        void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
        void Update();
    }
}
