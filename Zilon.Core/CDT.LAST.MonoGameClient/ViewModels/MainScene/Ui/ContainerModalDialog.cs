using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class ContainerModalDialog : ModalDialogBase
    {
        public ContainerModalDialog(IUiContentStorage uiContentStorage, GraphicsDevice graphicsDevice) : base(uiContentStorage, graphicsDevice)
        {
        }
    }
}
