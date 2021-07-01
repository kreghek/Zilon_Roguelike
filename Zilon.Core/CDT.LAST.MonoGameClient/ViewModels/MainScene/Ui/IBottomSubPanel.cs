using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public interface IBottomSubPanel
    {
        void Draw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.Rectangle contentRect);
        void Update();
        void UnsubscribeEvents();
    }
}