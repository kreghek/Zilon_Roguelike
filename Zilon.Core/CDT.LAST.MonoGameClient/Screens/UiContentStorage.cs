using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal sealed class UiContentStorage : IUiContentStorage
    {
        private readonly IDictionary<string, Texture2D[]> _propIcons;

        private Texture2D? _attributeIconsTexture;
        private Texture2D? _attributesBackgroundTexture;
        private SpriteFont? _auxFont;
        private SpriteFont _scoresFont;
        private Texture2D? _bottomPanelBackground;
        private SpriteFont? _buttonFont;
        private Texture2D? _buttonTexture;

        private IDictionary<string, Texture2D>? _combatActDict;
        private Texture2D? _conditionDefaultIcon;
        private Texture2D? _conditionDeseaseSymptomIcon;
        private Texture2D[]? _conditionIconBackgroundTextures;
        private Texture2D? _contextualMenuBorderTexture;
        private Texture2D? _contextualMenuItemBackgroundTexture;
        private SpriteFont? _contextualMenuItemFont;

        private Texture2D? _hintBackgorundTexture;
        private SpriteFont? _hintTitleFont;
        private Texture2D[]? _modalBottomTextures;
        private Texture2D? _modalShadowTexture;
        private Texture2D[]? _modalTopTextures;
        private Texture2D? _personMarkerTextureSheet;
        private Texture2D? _selectedButtonMarker;
        private Texture2D? _smallVerticalButtonBackgroundTexture;
        private Texture2D? _smallVerticalButtonIconsTexture;

        private IDictionary<SurvivalStatType, Texture2D>? _survivalStatConditionIcons;
        private Texture2D? _hexMarkersTexture;

        public UiContentStorage()
        {
            _propIcons = new Dictionary<string, Texture2D[]>();
        }

        private static Texture2D GetStatHazardConditionLevelClient(SurvivalStatHazardLevel level,
            Texture2D[] orderedTextures)
        {
            var defaultBackground = orderedTextures[0];

            switch (level)
            {
                case SurvivalStatHazardLevel.Lesser:
                    return orderedTextures[0];
                case SurvivalStatHazardLevel.Strong:
                    return orderedTextures[1];
                case SurvivalStatHazardLevel.Max:
                    return orderedTextures[2];
                case SurvivalStatHazardLevel.Undefined:
                    Debug.Fail("Undefined value can't be assigned.");
                    return defaultBackground;
                default:
                    Debug.Fail($"Value {level} is unknown.");
                    return defaultBackground;
            }
        }

        private Texture2D GetStatHazardConditionTypeClient(SurvivalStatType type)
        {
            if (_survivalStatConditionIcons is null)
            {
                throw new InvalidOperationException("Survival condition icons is not loaded.");
            }

            if (!_survivalStatConditionIcons.TryGetValue(type, out var iconTexture))
            {
                Debug.Fail("Every stat must has its own icon.");
            }

            return iconTexture;
        }

        private void InitCombatActIcons(ContentManager contentManager)
        {
            Texture2D loadIcon(string name) { return contentManager.Load<Texture2D>($"Sprites/ui/CombatActIcons/{name}"); };

            _combatActDict = new Dictionary<string, Texture2D>
            {
                ["default"] = loadIcon("SwordCut"),

                ["tag-punch"] = loadIcon("Punch"),

                ["uppercut"] = loadIcon("Uppercut"),
                ["death-punch"] = loadIcon("DeathPunch"),

                ["slash"] = loadIcon("SwordCut"),
                ["evasion-slash"] = loadIcon("EvasionSlash"),

                ["pierce"] = loadIcon("NormalPierce"),

                ["shield-push"] = loadIcon("ShieldBash"),

                ["stab"] = loadIcon("Stab"),
                ["dirty-trick"] = loadIcon("DirtyTrick"),

                ["bludgeon"] = loadIcon("Bludgeon"),

                ["chop"] = loadIcon("AxeCut"),

                ["swing"] = loadIcon("Swing"),
            };
        }

        private void InitConditionIconsAndBackgrounds(ContentManager contentManager)
        {
            _survivalStatConditionIcons = new Dictionary<SurvivalStatType, Texture2D>
            {
                {
                    SurvivalStatType.Satiety,
                    contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/HungerConditionIcon")
                },
                {
                    SurvivalStatType.Hydration,
                    contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/ThristConditionIcon")
                },
                {
                    SurvivalStatType.Intoxication,
                    contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/IntoxicationConditionIcon")
                },
                {
                    SurvivalStatType.Health,
                    contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/InjureConditionIcon")
                },
                {
                    SurvivalStatType.Energy,
                    contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/EnergyConditionIcon")
                }
            };

            _conditionIconBackgroundTextures = new[]
            {
                contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/ConditionIconLesserBackground"),
                contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/ConditionIconStrongBackground"),
                contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/ConditionIconCriticalBackground")
            };

            _conditionDeseaseSymptomIcon =
                contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/DiseaseSymptomConditionIcon");
            _conditionDefaultIcon = contentManager.Load<Texture2D>("Sprites/ui/PersonConditions/DefaultIcon");
        }

        private void InitPropIcons(ContentManager contentManager)
        {
            // Place textures in order to display. Latest will display on the top.
            // Text above is obsolete. We use only one image to show icon right now.
            _propIcons.Add("short-sword",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/ShortSword")
                });
            _propIcons.Add("knife",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/Knife")
                });
            _propIcons.Add("cudgel",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/Cudgel")
                });
            _propIcons.Add("great-sword",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/GreatSword")
                });
            _propIcons.Add("combat-staff",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/CombatStaff")
                });
            _propIcons.Add("tribal-spear",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/Spear")
                });
            _propIcons.Add("wooden-shield",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/WoodenShield")
                });
            _propIcons.Add("battle-axe",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/BattleAxe")
                });
            _propIcons.Add("pick-axe",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/PickAxe")
                });
            _propIcons.Add("katana",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/Katana")
                });
            _propIcons.Add("shovel",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/Shovel")
                });
            _propIcons.Add("mace",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/WeaponIcons/Mace")
                });

            _propIcons.Add("work-clothes",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/ClothIcons/WorkClothes") });
            _propIcons.Add("traveler-camisole",
                new[]
                {
                    contentManager.Load<Texture2D>("Sprites/ui/ClothIcons/TravelerCamisole")
                });
            _propIcons.Add("knitted-hat",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/ClothIcons/KnittedHat") });
            _propIcons.Add("steel-helmet",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/ClothIcons/SteelHelmet") });
            _propIcons.Add("headband",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/ClothIcons/Headband") });
            _propIcons.Add("highlander-helmet",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/ClothIcons/HighlanderHelmet") });

            _propIcons.Add("med-kit",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/MedKit") });
            _propIcons.Add("water-bottle",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/Waterskin") });
            _propIcons.Add("packed-food",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/PackedFood") });

            // Aux
            _propIcons.Add("evil-pumpkin",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/EvilPumpkin") });
            _propIcons.Add("fire-opal-necklace",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/Necklace") });
            _propIcons.Add("higea-ring",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/Ring") });
            _propIcons.Add("hunger-moderator",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/HungerModerator") });
            _propIcons.Add("hydro-necklace",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/Necklace") });
            _propIcons.Add("iron-ring",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/Ring") });
            _propIcons.Add("law-codex",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/Book") });
            _propIcons.Add("power-shield-generator",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/PowerShieldGenerator") });
            _propIcons.Add("taurus-stim",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/StimPack") });
            _propIcons.Add("thrist-moderator",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/ThristModerator") });
            _propIcons.Add("med-mask",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/MedicalMask") });
            _propIcons.Add("med-gloves",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/PropIcons/MedicalGloves") });

            _propIcons.Add("HeadSlot",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/InventorySlots/Head") });
            _propIcons.Add("BodySlot",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/InventorySlots/Body") });
            _propIcons.Add("RightHandSlot",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/InventorySlots/RightHand") });
            _propIcons.Add("LeftHandSlot",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/InventorySlots/LeftHand") });
            _propIcons.Add("AuxSlot",
                new[] { contentManager.Load<Texture2D>("Sprites/ui/InventorySlots/Auxiliary") });

            _propIcons.Add("EmptyPropIcon", new[] { contentManager.Load<Texture2D>("Sprites/ui/EmptyPropIcon") });
        }

        public Texture2D GetBottomPanelBackground()
        {
            return _bottomPanelBackground ??
                   throw new InvalidOperationException("Bottom panel background texture is not loaded.");
        }

        public Texture2D GetSmallVerticalButtonBackgroundTexture()
        {
            return _smallVerticalButtonBackgroundTexture ?? throw new InvalidOperationException();
        }

        public SpriteFont GetButtonFont()
        {
            return _buttonFont ?? throw new InvalidOperationException();
        }

        public Texture2D GetButtonTexture()
        {
            return _buttonTexture ?? throw new InvalidOperationException();
        }

        public Texture2D[] GetModalBottomTextures()
        {
            return _modalBottomTextures ?? throw new InvalidOperationException();
        }

        public Texture2D[] GetModalTopTextures()
        {
            return _modalTopTextures ?? throw new InvalidOperationException();
        }

        public Texture2D[] GetPropIconLayers(string sid)
        {
            if (_propIcons.TryGetValue(sid, out var propTextureList))
            {
                return propTextureList;
            }

            Debug.Fail($"Each prop in the game must have your own icon. {sid} hasn't it.");
            return _propIcons["EmptyPropIcon"];
        }

        public Texture2D GetModalShadowTexture()
        {
            return _modalShadowTexture ?? throw new InvalidOperationException();
        }

        public Texture2D GetSelectedButtonMarkerTexture()
        {
            return _selectedButtonMarker ?? throw new InvalidOperationException("Button markers is not loaded.");
        }

        public void LoadContent(ContentManager contentManager)
        {
            _buttonFont = contentManager.Load<SpriteFont>("Fonts/Main");
            _hintTitleFont = contentManager.Load<SpriteFont>("Fonts/HintTitle");
            _auxFont = contentManager.Load<SpriteFont>("Fonts/UiAux");
            _scoresFont = contentManager.Load<SpriteFont>("Fonts/Scores");
            _hintBackgorundTexture = contentManager.Load<Texture2D>("Sprites/ui/HintBackground");
            _buttonTexture = contentManager.Load<Texture2D>("Sprites/ui/button");
            _modalShadowTexture = contentManager.Load<Texture2D>("Sprites/ui/ModalDialogShadow");
            _modalTopTextures = new[] { contentManager.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundTop1") };
            _modalBottomTextures = new[] { contentManager.Load<Texture2D>("Sprites/ui/ModalDialogBackgroundBottom1") };
            _attributeIconsTexture = contentManager.Load<Texture2D>("Sprites/ui/AttributeIcons");
            _attributesBackgroundTexture = contentManager.Load<Texture2D>("Sprites/ui/AttributesBackground");
            _smallVerticalButtonIconsTexture = contentManager.Load<Texture2D>("Sprites/ui/SmallVerticalButtonIcons");
            _smallVerticalButtonBackgroundTexture =
                contentManager.Load<Texture2D>("Sprites/ui/SmallVerticalButtonBackground");
            _bottomPanelBackground = contentManager.Load<Texture2D>("Sprites/ui/BottomPanelBackground");
            _selectedButtonMarker = contentManager.Load<Texture2D>("Sprites/ui/SelectedButtonMarker");
            _personMarkerTextureSheet = contentManager.Load<Texture2D>("Sprites/ui/PersonMarkers");

            _contextualMenuItemFont = contentManager.Load<SpriteFont>("Fonts/ContextualMenu");
            _contextualMenuBorderTexture = contentManager.Load<Texture2D>("Sprites/ui/ContextualMenuBorder");
            _contextualMenuItemBackgroundTexture =
                contentManager.Load<Texture2D>("Sprites/ui/ContextualMenuItemBackground");
            _hexMarkersTexture = contentManager.Load<Texture2D>("Sprites/ui/HexMarkers");

            InitPropIcons(contentManager);
            InitCombatActIcons(contentManager);
            InitConditionIconsAndBackgrounds(contentManager);
        }

        public SpriteFont GetHintTitleFont()
        {
            return _hintTitleFont ?? throw new InvalidOperationException();
        }

        public Texture2D GetAttributeIconsTexture()
        {
            return _attributeIconsTexture ?? throw new InvalidOperationException();
        }

        public Texture2D GetSmallVerticalButtonIconsTexture()
        {
            return _smallVerticalButtonIconsTexture ?? throw new InvalidOperationException();
        }

        public Texture2D GetAttributeBackgroundTexture()
        {
            return _attributesBackgroundTexture ?? throw new InvalidOperationException();
        }

        public Texture2D GetContextualMenuBorderTexture()
        {
            return _contextualMenuBorderTexture ?? throw new InvalidOperationException();
        }

        public SpriteFont GetMenuItemFont()
        {
            return _contextualMenuItemFont ?? throw new InvalidOperationException();
        }

        public Texture2D GetMenuItemTexture()
        {
            return _contextualMenuItemBackgroundTexture ?? throw new InvalidOperationException();
        }

        public Texture2D GetHexMarkers()
        {
            return _hexMarkersTexture ?? throw new InvalidOperationException();
        }

        public PersonConditionTextures GetConditionIconTextures(IPersonCondition personCondition)
        {
            if (_conditionIconBackgroundTextures is null)
            {
                throw new InvalidOperationException("Icon backgrounds are not loaded.");
            }

            switch (personCondition)
            {
                case SurvivalStatHazardCondition statCondition:

                    var typeTexture = GetStatHazardConditionTypeClient(statCondition.Type);
                    var levelBackgroundTexture =
                        GetStatHazardConditionLevelClient(statCondition.Level, _conditionIconBackgroundTextures);
                    return new PersonConditionTextures(typeTexture, levelBackgroundTexture);

                case DiseaseSymptomCondition:

                    if (_conditionDeseaseSymptomIcon is null)
                    {
                        throw new InvalidOperationException("Desease icon is not loaded.");
                    }

                    return new PersonConditionTextures(_conditionDeseaseSymptomIcon,
                        _conditionIconBackgroundTextures[0]);

                default:
                    Debug.Fail("Every condition must have icon.");
                    return new PersonConditionTextures(_conditionDefaultIcon, _conditionIconBackgroundTextures[0]);
            }
        }

        public Texture2D GetHintBackgroundTexture()
        {
            return _hintBackgorundTexture ?? throw new InvalidOperationException("Background texture is not loaded.");
        }

        public Texture2D GetCombatActIconTexture(string? sid, string[] tags)
        {
            if (_combatActDict is null)
            {
                throw new InvalidOperationException("Combat act textures are not loaded.");
            }

            if (sid is not null && _combatActDict.TryGetValue(sid, out var texture))
            {
                return texture;
            }

            foreach (var tag in tags)
            {
                if (_combatActDict.TryGetValue($"tag-{tag}", out texture))
                {
                    return texture;
                }
            }

            Debug.Fail("Every combat act must has own icon.");

            return _combatActDict["default"];
        }

        public SpriteFont GetAuxTextFont()
        {
            return _auxFont ?? throw new InvalidOperationException("Aux font was not loaded.");
        }

        public Texture2D GetPersonMarkerTextureSheet()
        {
            return _personMarkerTextureSheet ??
                   throw new InvalidOperationException("Person marker texture was not loaded.");
        }

        public SpriteFont GetScoresFont()
        {
            return _scoresFont ?? throw new InvalidOperationException("Scores font was not loaded.");
        }
    }
}