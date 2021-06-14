using System.Linq;

using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.PersonModules;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class CombatActPanel
    {
        private readonly ICombatActModule _combatActModule;
        private readonly IUiContentStorage _uiContentStorage;

        public CombatActPanel(ICombatActModule combatActModule, IUiContentStorage uiContentStorage)
        {
            _combatActModule = combatActModule;
            _uiContentStorage = uiContentStorage;
        }

        public void Update()
        { 
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var acts = _combatActModule.CalcCombatActs();
            var actsOrdered = acts.OrderBy(x => x.Scheme?.Sid).ToArray();
            var actIndex = 0;
            foreach (var act in actsOrdered)
            {
                const int BUTTON_SIZE = 32;
                const int BOTTOM_PANEL_HEIGHT = 32;
                var buttonrect = new Rectangle(actIndex * BUTTON_SIZE, graphicsDevice.Viewport.Bounds.Bottom - BUTTON_SIZE - BOTTOM_PANEL_HEIGHT, BUTTON_SIZE, BUTTON_SIZE);
                spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), buttonrect, Color.White);
                    actIndex++;
            }
        }
    }
}
