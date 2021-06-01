using System.Collections.Generic;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.PersonModules;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class PersonEquipmentModalDialog : ModalDialog
    {
        private readonly IUiContentStorage _uiContentStorage;
        private readonly IEquipmentModule _equipmentModule;

        private readonly IconButton[] _currentEquipmentButtons;

        public PersonEquipmentModalDialog(
            IUiContentStorage uiContentStorage,
            GraphicsDevice graphicsDevice,
            IEquipmentModule equipmentModule) : base(uiContentStorage, graphicsDevice)
        {
            _uiContentStorage = uiContentStorage;
            _equipmentModule = equipmentModule;

            var currentEquipmentButtonList = new List<IconButton>();
            foreach (var equipment in _equipmentModule)
            {
                if (equipment is null)
                {
                    continue;
                }

                var equipmentButton = new IconButton(
                    equipment.Scheme.Name?.En ?? "<Undef>",
                    _uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetPropIcon("test"),
                    new Rectangle(currentEquipmentButtonList.Count * 32 + ContentRect.Left, ContentRect.Top, 32, 32));

                currentEquipmentButtonList.Add(equipmentButton);
            }

            _currentEquipmentButtons = currentEquipmentButtonList.ToArray();
        }

        protected override void DrawContent(SpriteBatch spriteBatch, Rectangle contentRect)
        {
            foreach (var button in _currentEquipmentButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        protected override void UpdateContent()
        {
            foreach (var button in _currentEquipmentButtons)
            {
                button.Update();
            }
        }
    }
}
