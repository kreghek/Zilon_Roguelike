using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class PersonStatsModalDialog : ModalDialog
    {
        private readonly ISectorUiState _uiState;

        public PersonStatsModalDialog(IUiContentStorage uiContentStorage, GraphicsDevice graphicsDevice, ISectorUiState uiState) : base(uiContentStorage, graphicsDevice)
        {
            _uiState = uiState;
        }

        /// <inheritdoc/>
        protected override void DrawContent(SpriteBatch spriteBatch)
        {
            base.DrawContent(spriteBatch);
        }

        protected override void InitContent()
        {
            base.InitContent();


        }
    }
}
