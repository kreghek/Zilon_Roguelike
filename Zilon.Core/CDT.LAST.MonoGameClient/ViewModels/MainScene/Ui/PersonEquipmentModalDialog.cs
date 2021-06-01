using System.Collections.Generic;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.PersonModules;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class PersonEquipmentModalDialog : ModalDialog
    {
        private readonly IEquipmentModule _equipmentModule;

        private readonly Button[] _currentEquipmentButtons;

        public PersonEquipmentModalDialog(
            Texture2D backgroundTopTexture,
            Texture2D backgroundBottomTexture,
            Texture2D shadowTexture,
            Texture2D buttonTexture,
            SpriteFont font,
            GraphicsDevice graphicsDevice,
            IEquipmentModule equipmentModule) : base(backgroundTopTexture, backgroundBottomTexture, shadowTexture, buttonTexture, font, graphicsDevice)
        {
            _equipmentModule = equipmentModule;

            var currentEquipmentButtonList = new List<Button>();
            foreach (var equipment in _equipmentModule)
            {
                if (equipment is null)
                {
                    continue;
                }

                var equipmentButton = new Button(
                    equipment.Scheme.Name?.En ?? "<Undef>",
                    buttonTexture,
                    font,
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
