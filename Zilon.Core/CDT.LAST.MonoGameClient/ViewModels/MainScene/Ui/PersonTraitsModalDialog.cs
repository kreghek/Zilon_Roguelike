﻿using System;
using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Components;
using Zilon.Core.Localization;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

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
                DrawPerk(item, spriteBatch);
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
            var colWidth = ContentRect.Width / 2;

            var MAX_ITEM_IN_ROW = 8;
            var MAX_ITEM_COUNT = MAX_ITEM_IN_ROW * 2;

            var profession = CreateProfession(
                ((HumanPerson)person).PersonEquipmentTemplate,
                ((HumanPerson)person).PersonEquipmentDescriptionTemplate);

            var perks = evolutionModule.GetArchievedPerks().Take(MAX_ITEM_COUNT).ToArray();

            var allPerks = new[] { profession }.Concat(perks).ToArray();

            for (var itemIndex = 0; itemIndex < allPerks.Length; itemIndex++)
            {
                var perk = allPerks[itemIndex];
                var colIndex = itemIndex / MAX_ITEM_IN_ROW;
                var rowIndex = itemIndex % MAX_ITEM_IN_ROW;
                var relativeX = colWidth * colIndex;
                var relativeY = rowIndex * (ATTRIBUTE_ITEM_SIZE + ATTRIBUTE_ITEM_SPACING);
                var uiRect = new Rectangle(
                    ContentRect.Left + relativeX,
                    ContentRect.Top + relativeY,
                    ATTRIBUTE_ITEM_SIZE,
                    ATTRIBUTE_ITEM_SIZE);
                var uiItem = new PersonPerkUiItem(perk, itemIndex, uiRect);
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

        private IPerk CreateProfession(ILocalizedString? personEquipmentTemplate,
            ILocalizedString? personEquipmentDescriptionTemplate)
        {
            var profession = new ProfessionPerk(personEquipmentTemplate, personEquipmentDescriptionTemplate);
            return profession;
        }

        private void DrawHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_selectedAttributeItem is null)
            {
                return;
            }

            var perkDescription = GetPerkDescription(_selectedAttributeItem.Perk);
            var hintTitleFont = _uiContentStorage.GetHintTitleFont();
            var titleTextSizeVector = hintTitleFont.MeasureString(perkDescription);

            const int HINT_TEXT_SPACING = 8;
            var hintRectangle = new Rectangle(
                _selectedAttributeItem.UiRect.Left,
                _selectedAttributeItem.UiRect.Bottom + ATTRIBUTE_ITEM_SPACING,
                (int)titleTextSizeVector.X + (HINT_TEXT_SPACING * 2),
                (int)titleTextSizeVector.Y + (HINT_TEXT_SPACING * 2));

            spriteBatch.Draw(_uiContentStorage.GetButtonTexture(), hintRectangle, Color.DarkSlateGray);

            spriteBatch.DrawString(hintTitleFont, perkDescription,
                new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                Color.Wheat);
        }

        private void DrawPerk(PersonPerkUiItem item, SpriteBatch spriteBatch)
        {
            var sourceRect = GetPerkIcon(item.Perk);
            spriteBatch.Draw(_uiContentStorage.GetAttributeBackgroundTexture(),
                new Vector2(item.UiRect.Left, item.UiRect.Top), Color.White);
            spriteBatch.Draw(_uiContentStorage.GetAttributeIconsTexture(),
                new Vector2(item.UiRect.Left, item.UiRect.Top), sourceRect, Color.White);

            var attributeTitle = GetPerkTitle(item.Perk);
            spriteBatch.DrawString(
                _uiContentStorage.GetButtonFont(),
                attributeTitle,
                new Vector2(item.UiRect.Right + ATTRIBUTE_ITEM_SPACING, item.UiRect.Top),
                new Color(195, 180, 155));
        }

        private static string GetPerkDescription(IPerk perk)
        {
            return PerkHelper.GetPerkHintText(perk);
        }

        private static Rectangle GetPerkIcon(IPerk perk)
        {
            if (perk.Scheme.IsBuildIn)
            {
                return new Rectangle(0, 64, 32, 32);
            }

            return new Rectangle(32, 64, 32, 32);
        }

        private static string GetPerkTitle(IPerk perk)
        {
            return PerkHelper.GetPropTitle(perk);
        }

        private sealed class ProfessionPerk : IPerk
        {
            public ProfessionPerk(ILocalizedString personEquipmentTemplate,
                ILocalizedString descriptionEquipmentTemplate)
            {
                Scheme = new PersonPerkScheme
                {
                    Name = new LocalizedStringSubScheme
                    {
                        Ru = personEquipmentTemplate.Ru,
                        En = personEquipmentTemplate.En
                    },
                    Description = new LocalizedStringSubScheme
                    {
                        Ru = descriptionEquipmentTemplate.Ru,
                        En = descriptionEquipmentTemplate.En
                    }
                };
            }

            public PerkLevel? CurrentLevel { get; set; }
            public IPerkScheme Scheme { get; }
            public IJob[]? CurrentJobs { get; }
        }

        private sealed class PersonPerkScheme : IPerkScheme
        {
            public PerkConditionSubScheme?[]? BaseConditions { get; set; }
            public string? IconSid { get; set; }
            public bool IsBuildIn => true;
            public JobSubScheme?[]? Jobs { get; set; }
            public PerkLevelSubScheme?[]? Levels { get; set; }
            public int Order { get; set; }
            public PerkRuleSubScheme?[]? Rules { get; set; }
            public PropSet?[]? Sources { get; set; }
            public PerkConditionSubScheme?[]? VisibleConditions { get; set; }
            public LocalizedStringSubScheme? Description { get; set; }
            public bool Disabled { get; }
            public LocalizedStringSubScheme? Name { get; set; }
            public string? Sid { get; set; }
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