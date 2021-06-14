using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class CombatActPanel
    {
        private readonly ICombatActModule _combatActModule;
        private readonly IEquipmentModule _equipmentModule;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _sectorUiState;
        private readonly IList<IconButton> _buttons;

        public CombatActPanel(ICombatActModule combatActModule, IEquipmentModule equipmentModule, IUiContentStorage uiContentStorage, ISectorUiState sectorUiState)
        {
            _combatActModule = combatActModule;
            _equipmentModule = equipmentModule;
            _uiContentStorage = uiContentStorage;
            _sectorUiState = sectorUiState;

            _buttons = new List<IconButton>();

            Initialize(_buttons);

            _equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
        }

        private void Initialize(IList<IconButton> _buttons)
        {
            var acts = _combatActModule.CalcCombatActs();
            var actsOrdered = acts.OrderBy(x => x.Scheme?.Sid).ToArray();
            foreach (var act in actsOrdered)
            {
                const int BUTTON_SIZE = 32;
                var button = new IconButton(_uiContentStorage.GetButtonTexture(), _uiContentStorage.GetPropIconLayers("short-sword")[0],
                    new Rectangle(0, 0, BUTTON_SIZE, BUTTON_SIZE));
                button.OnClick += (s, e) =>
                {
                    _sectorUiState.TacticalAct = act;
                };
                _buttons.Add(button);
            }
        }

        public void Update()
        {
            foreach (var button in _buttons)
            {
                button.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            var acts = _combatActModule.CalcCombatActs();
            var actsOrdered = acts.OrderBy(x => x.Scheme?.Sid).ToArray();
            var actIndex = 0;
            foreach (var button in _buttons)
            {
                const int BUTTON_SIZE = 32;
                const int BOTTOM_PANEL_HEIGHT = 32;
                var buttonRect = new Rectangle(actIndex * BUTTON_SIZE, graphicsDevice.Viewport.Bounds.Bottom - BUTTON_SIZE - BOTTOM_PANEL_HEIGHT, BUTTON_SIZE, BUTTON_SIZE);

                button.Rect = buttonRect;

                button.Draw(spriteBatch);
            }
        }

        public void UnsubscribeEvents()
        {
            _equipmentModule.EquipmentChanged -= EquipmentModule_EquipmentChanged;
        }

        private void EquipmentModule_EquipmentChanged(object? sender, Zilon.Core.Persons.EquipmentChangedEventArgs e)
        {
            _buttons.Clear();
            Initialize(_buttons);
        }
    }
}
