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

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    internal sealed class PersonStatsModalDialog : ModalDialog
    {
        private const int ATTRIBUTE_ITEM_SIZE = 32;
        private const int ATTRIBUTE_ITEM_SPACING = 2;

        private readonly IUiContentStorage _uiContentStorage;
        private readonly ISectorUiState _uiState;
        private PersonStatUiItem[]? _currentAttributesItems;
        private PersonStatUiItem? _selectedAttributeItem;

        public PersonStatsModalDialog(IUiContentStorage uiContentStorage, GraphicsDevice graphicsDevice,
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

            var attributesModule = person.GetModuleSafe<IAttributesModule>();
            if (attributesModule is null)
            {
                throw new InvalidOperationException(
                    "Active person must have attributes to shown in this dialog.");
            }

            var currentAttributeItemList = new List<PersonStatUiItem>();

            var attributes = attributesModule.GetAttributes();
            foreach (var attribute in attributes)
            {
                var lastIndex = currentAttributeItemList.Count;
                var relativeY = (lastIndex * ATTRIBUTE_ITEM_SIZE) + ATTRIBUTE_ITEM_SPACING;
                var uiRect = new Rectangle(
                    ContentRect.Left,
                    ContentRect.Top + relativeY,
                    ATTRIBUTE_ITEM_SIZE,
                    ATTRIBUTE_ITEM_SIZE);
                var uiItem = new PersonStatUiItem(attribute, lastIndex, uiRect);
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

        private void DrawAttribute(PersonStatUiItem item, SpriteBatch spriteBatch)
        {
            var sourceRect = GetAttributeIcon(item.Attribute.Type);
            spriteBatch.Draw(_uiContentStorage.GetAttributeBackgroundTexture(),
                new Vector2(item.UiRect.Left, item.UiRect.Top), Color.White);
            spriteBatch.Draw(_uiContentStorage.GetAttributeIconsTexture(),
                new Vector2(item.UiRect.Left, item.UiRect.Top), sourceRect, Color.White);

            var attributeTitle = GetAttributeTitle(item.Attribute);
            var attributeValue = GetAttributeTextValue(item.Attribute);
            spriteBatch.DrawString(
                _uiContentStorage.GetButtonFont(),
                $"{attributeTitle}: {attributeValue}",
                new Vector2(item.UiRect.Right + ATTRIBUTE_ITEM_SPACING, item.UiRect.Top),
                new Color(195, 180, 155));
        }

        private void DrawHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_selectedAttributeItem is null)
            {
                return;
            }

            var attributeDescription = GetAttributeDescription(_selectedAttributeItem.Attribute);
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

        private static string GetAttributeDescription(PersonAttribute attribute)
        {
            switch (attribute.Type)
            {
                case PersonAttributeType.PhysicalStrength:
                    return UiResources.PhysicalStrengthAttributeDescription;

                case PersonAttributeType.Dexterity:
                    return UiResources.DexterityAttributeDescription;

                case PersonAttributeType.Perception:
                    return UiResources.PerceptionAttributeDescription;

                case PersonAttributeType.Constitution:
                    return UiResources.ConstitutionAttributeDescription;

                default:
                    Debug.Fail($"Unknown attribute {attribute.Type}.");
                    return "<Undef>";
            }
        }

        private static Rectangle GetAttributeIcon(PersonAttributeType type)
        {
            switch (type)
            {
                case PersonAttributeType.PhysicalStrength:
                    return new Rectangle(0, 0, 32, 32);

                case PersonAttributeType.Dexterity:
                    return new Rectangle(32, 0, 32, 32);

                case PersonAttributeType.Perception:
                    return new Rectangle(0, 32, 32, 32);

                case PersonAttributeType.Constitution:
                    return new Rectangle(32, 32, 32, 32);

                default:
                    Debug.Fail($"Unknown attribute {type}.");
                    return new Rectangle(0, 0, 1, 1);
            }
        }

        private static string GetAttributeTextValue(PersonAttribute attribute)
        {
            switch (attribute.Value)
            {
                case float n when n is >= 0 and <= 1: return UiResources.UbnormalAttributeValueTitle;
                case float n when n is >= 2 and <= 3: return UiResources.DefectiveAttributeValueTitle;
                case float n when n is >= 4 and <= 5: return UiResources.LowAttributeValueTitle;
                case float n when n is >= 6 and <= 8: return UiResources.BelowNormalAttributeValueTitle;
                case float n when n is >= 9 and <= 11: return UiResources.NormalAttributeValueTitle;
                case float n when n is >= 12 and <= 13: return UiResources.AboveNormalAttributeValueTitle;
                case float n when n is >= 14 and <= 15: return UiResources.SuperAttributeValueTitle;
                default:
                    Debug.Fail("The attribute value can't be out of defined range.");
                    return "<Unknown>";
            }
        }

        private static string GetAttributeTitle(PersonAttribute attribute)
        {
            switch (attribute.Type)
            {
                case PersonAttributeType.PhysicalStrength:
                    return UiResources.PhysicalStrengthAttributeTitle;

                case PersonAttributeType.Dexterity:
                    return UiResources.DexterityAttributeTitle;

                case PersonAttributeType.Perception:
                    return UiResources.PerceptionAttributeTitle;

                case PersonAttributeType.Constitution:
                    return UiResources.ConstitutionAttributeTitle;

                default:
                    Debug.Fail($"Unknown attribute {attribute.Type}.");
                    return "<Undef>";
            }
        }

        private record PersonStatUiItem
        {
            public PersonStatUiItem(PersonAttribute attribute, int uiIndex, Rectangle uiRect)
            {
                Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
                UiIndex = uiIndex;
                UiRect = uiRect;
            }

            public PersonAttribute Attribute { get; }

            public int UiIndex { get; }
            public Rectangle UiRect { get; }
        }
    }
}