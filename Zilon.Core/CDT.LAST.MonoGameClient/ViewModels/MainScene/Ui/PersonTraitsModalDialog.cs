using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Resources;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class PersonTraitsModalDialog : ModalDialogBase
    {
        private const int ATTRIBUTE_ITEM_SIZE = 32;
        private const int ATTRIBUTE_ITEM_SPACING = 2;

        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;
        private PersonPerkUiItem[]? _currentAttributesItems;
        private PersonPerkUiItem? _selectedAttributeItem;

        public PersonTraitsModalDialog(IUiContentStorage uiContentStorage, GraphicsDevice graphicsDevice,
            ISectorUiState uiState) : base(uiContentStorage, graphicsDevice)
        {
            _uiContentStorage = uiContentStorage;
            _uiState = uiState;
        }

        /// <inheritdoc />
        protected override void DrawContent(SpriteBatch spriteBatch)
        {
            if (_currentAttributesItems is null)
            {
                return;
            }

            foreach (var item in _currentAttributesItems)
            {
                DrawAttribute(item, spriteBatch);
            }

            DrawHintIfSelected(spriteBatch);
        }

        /// <inheritdoc />
        protected override void InitContent()
        {
            base.InitContent();

            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                throw new InvalidOperationException("Active person must be selected before this dialog was opened.");
            }

            var evolutionModule = person.GetModuleSafe<IEvolutionModule>();
            if (evolutionModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must have attributes to shown in this dialog.");
            }

            var currentAttributeItemList = new List<PersonPerkUiItem>();

            var perks = evolutionModule.GetArchievedPerks();
            foreach (var perk in perks)
            {
                var lastIndex = currentAttributeItemList.Count;
                var relativeY = lastIndex * (ATTRIBUTE_ITEM_SIZE + ATTRIBUTE_ITEM_SPACING);
                var uiRect = new Rectangle(
                    ContentRect.Left,
                    ContentRect.Top + relativeY,
                    ATTRIBUTE_ITEM_SIZE,
                    ATTRIBUTE_ITEM_SIZE);
                var uiItem = new PersonPerkUiItem(perk, lastIndex, uiRect);
                currentAttributeItemList.Add(uiItem);
            }

            _currentAttributesItems = currentAttributeItemList.ToArray();
        }

        protected override void UpdateContent()
        {
            if (_currentAttributesItems is null)
            {
                return;
            }

            var mouseState = Mouse.GetState();

            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            var effectUnderMouse = _currentAttributesItems.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            _selectedAttributeItem = effectUnderMouse;
        }

        private void DrawAttribute(PersonPerkUiItem item, SpriteBatch spriteBatch)
        {
            var sourceRect = GetPerkIcon();
            spriteBatch.Draw(_uiContentStorage.GetAttributeBackgroundTexture(),
                new Vector2(item.UiRect.Left, item.UiRect.Top), Color.White);
            spriteBatch.Draw(_uiContentStorage.GetAttributeIconsTexture(),
                new Vector2(item.UiRect.Left, item.UiRect.Top), sourceRect, Color.White);

            var attributeTitle = GetAttributeTitle(item.Perk);
             spriteBatch.DrawString(
                _uiContentStorage.GetButtonFont(),
                attributeTitle,
                new Vector2(item.UiRect.Right + ATTRIBUTE_ITEM_SPACING, item.UiRect.Top),
                new Color(195, 180, 155));
        }

        private void DrawHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_selectedAttributeItem is null)
            {
                return;
            }

            var attributeDescription = GetAttributeDescription(_selectedAttributeItem.Perk);
            var hintTitleFont = _uiContentStorage.GetHintTitleFont();
            var titleTextSizeVector = hintTitleFont.MeasureString(attributeDescription);

            const int HINT_TEXT_SPACING = 8;
            var hintRectangle = new Rectangle(
                _selectedAttributeItem.UiRect.Left,
                _selectedAttributeItem.UiRect.Bottom + ATTRIBUTE_ITEM_SPACING,
                (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

            spriteBatch.DrawString(hintTitleFont, attributeDescription,
                new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                Color.Wheat);
        }

        private static string GetAttributeDescription(IPerk perk)
        {
            return perk.Scheme.Description?.En ?? perk.Scheme.Description?.Ru ?? string.Empty;
        }

        private static Rectangle GetPerkIcon()
        {
            return new Rectangle(0, 64, 32, 32);
        }

        private static string GetAttributeTitle(IPerk perk)
        {
            return perk.Scheme?.Name?.En;
        }

        private record PersonPerkUiItem
        {
            public PersonPerkUiItem(IPerk perk, int uiIndex, Rectangle uiRect)
            {
                Perk = perk ?? throw new ArgumentNullException(nameof(perk));
                UiIndex = uiIndex;
                UiRect = uiRect;
            }

            public IPerk Perk { get; }

            public int UiIndex { get; }
            public Rectangle UiRect { get; }
        }
    }
}