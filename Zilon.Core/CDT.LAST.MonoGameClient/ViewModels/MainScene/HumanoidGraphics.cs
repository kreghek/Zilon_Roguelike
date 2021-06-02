using System;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class HumanoidGraphics : SpriteContainer, IActorGraphics
    {
        private readonly Texture2D _armLeftTexture;
        private readonly Texture2D _armLeftTravelerTexture;
        private readonly Texture2D _armRightTexture;
        private readonly Texture2D _armRightTravelerTexture;
        private readonly Texture2D _bodyClothsTexture;
        private readonly Texture2D _bodyTexture;
        private readonly Texture2D _bodyTravelerTexture;
        private readonly IEquipmentModule _equipmentModule;

        private readonly Texture2D _headTexture;
        private readonly Texture2D _legsTexture;
        private readonly Texture2D _legsTravelerTexture;
        private readonly Texture2D _shieldBaseTexture;
        private readonly Texture2D _weaponBaseTexture;

        public HumanoidGraphics(IEquipmentModule equipmentModule, ContentManager contentManager)
        {
            _equipmentModule = equipmentModule;

            _headTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Human/Head");
            _bodyTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Human/Body");
            _legsTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Human/LegsIdle");
            _armLeftTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Human/ArmLeftSimple");
            _armRightTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Human/ArmRightSimple");

            _bodyClothsTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/CasualCloths/Body");

            _bodyTravelerTexture =
                contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/TravellerCloths/Body");
            _legsTravelerTexture =
                contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/TravellerCloths/LegsIdle");
            _armLeftTravelerTexture =
                contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/TravellerCloths/ArmLeftSimple");
            _armRightTravelerTexture =
                contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/TravellerCloths/ArmRightSimple");

            _weaponBaseTexture =
                contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/HandEquiped/ShortSwordBase");
            _shieldBaseTexture =
                contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/HandEquiped/WoodenShieldBase");

            CreateSpriteHierarchy(equipmentModule);

            equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
        }

        private void CreateSpriteHierarchy(IEquipmentModule equipmentModule)
        {
            DrawLeftHand(equipmentModule, _armLeftTexture, _weaponBaseTexture, _shieldBaseTexture);

            AddChild(new Sprite(_legsTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.75f)
            });

            DrawBody(equipmentModule);

            AddChild(new Sprite(_headTexture)
            {
                Position = new Vector2(-0, -20),
                Origin = new Vector2(0.5f, 1)
            });

            DrawRightHand(equipmentModule, _armRightTexture, _weaponBaseTexture, _shieldBaseTexture);
        }

        private void DrawBody(IEquipmentModule equipmentModule)
        {
            AddChild(new Sprite(_bodyTexture)
            {
                Position = new Vector2(3, -22),
                Origin = new Vector2(0.5f, 0.5f)
            });

            // This slot is body.
            var equipment = equipmentModule[1];
            if (equipment != null)
            {
                if (equipment.Scheme.Sid == "cloths")
                {
                    AddChild(new Sprite(_bodyClothsTexture)
                    {
                        Position = new Vector2(3, -22),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
                else if (equipment.Scheme.Sid == "traveler")
                {
                    AddChild(new Sprite(_bodyTravelerTexture)
                    {
                        Position = new Vector2(3, -22),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
            }
        }

        private void DrawLeftHand(IEquipmentModule equipmentModule, Texture2D armLeftTexture,
            Texture2D weaponBaseTexture, Texture2D shieldBackBaseTexture)
        {
            // Slot 3 according the person scheme is second/left hand.
            var equipment = equipmentModule[2];
            if (equipment != null)
            {
                var equipmentTags = equipment.Scheme.Tags;
                if (equipmentTags is null)
                {
                    // Now a person can equiped only weapons or tools.
                    // All weapons or tools has corresponding tags.
                    Debug.Fail("There is no scenario then equipment has no tags.");

                    AddChild(new Sprite(armLeftTexture)
                    {
                        Position = new Vector2(-10, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
                else if (equipmentTags.Contains(PropTags.Equipment.Weapon))
                {
                    AddChild(new Sprite(armLeftTexture)
                    {
                        Position = new Vector2(-10, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });

                    AddChild(new Sprite(weaponBaseTexture)
                    {
                        Position = new Vector2(-14, -21),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
                else if (equipmentTags.Contains(PropTags.Equipment.Shield))
                {
                    // For a shield in this hand draw shield back sprite first.
                    // So it will looks like the person bear shield by inner side.

                    AddChild(new Sprite(shieldBackBaseTexture)
                    {
                        Position = new Vector2(-14, -21),
                        Origin = new Vector2(0.5f, 0.5f)
                    });

                    AddChild(new Sprite(armLeftTexture)
                    {
                        Position = new Vector2(-10, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
                else
                {
                    AddChild(new Sprite(armLeftTexture)
                    {
                        Position = new Vector2(13, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
            }
            else
            {
                AddChild(new Sprite(armLeftTexture)
                {
                    Position = new Vector2(13, -20),
                    Origin = new Vector2(0.5f, 0.5f)
                });
            }
        }

        private void DrawRightHand(IEquipmentModule equipmentModule, Texture2D armRightTexture,
            Texture2D weaponBaseTexture, Texture2D shieldBaseTexture)
        {
            // Slot 3 according the person scheme is second/left hand.
            var weaponEquipment = equipmentModule[3];
            var bodyEquipment = equipmentModule[1];
            if (weaponEquipment != null)
            {
                var equipmentTags = weaponEquipment.Scheme.Tags;
                if (equipmentTags is null)
                {
                    // Now a person can equiped only weapons or tools.
                    // All weapons or tools has corresponding tags.
                    Debug.Fail("There is no scenario then equipment has no tags.");

                    AddChild(new Sprite(armRightTexture)
                    {
                        Position = new Vector2(13, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });

                    if (bodyEquipment != null)
                    {
                        if (bodyEquipment.Scheme.Sid == "traveler")
                        {
                            AddChild(new Sprite(_armRightTravelerTexture)
                            {
                                Position = new Vector2(13, -20),
                                Origin = new Vector2(0.5f, 0.5f)
                            });
                        }
                    }
                }
                else if (equipmentTags.Contains(PropTags.Equipment.Weapon))
                {
                    AddChild(new InvertedFlipXSprite(weaponBaseTexture)
                    {
                        Position = new Vector2(11, -10),
                        Origin = new Vector2(0.5f, 0.5f),
                        Rotation = (float)(-Math.PI / 2)
                    });

                    AddChild(new Sprite(armRightTexture)
                    {
                        Position = new Vector2(13, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });

                    if (bodyEquipment != null)
                    {
                        if (bodyEquipment.Scheme.Sid == "traveler")
                        {
                            AddChild(new Sprite(_armRightTravelerTexture)
                            {
                                Position = new Vector2(13, -20),
                                Origin = new Vector2(0.5f, 0.5f)
                            });
                        }
                    }
                }
                else if (equipmentTags.Contains(PropTags.Equipment.Shield))
                {
                    // For shield draw arm first because the base of the sheild
                    // should be cover the arm.

                    AddChild(new Sprite(armRightTexture)
                    {
                        Position = new Vector2(13, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });

                    if (bodyEquipment != null)
                    {
                        if (bodyEquipment.Scheme.Sid == "traveler")
                        {
                            AddChild(new Sprite(_armRightTravelerTexture)
                            {
                                Position = new Vector2(13, -20),
                                Origin = new Vector2(0.5f, 0.5f)
                            });
                        }
                    }

                    AddChild(new Sprite(shieldBaseTexture)
                    {
                        Position = new Vector2(11, -10),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
                else
                {
                    Debug.Fail("Unknown tag of thing in hand. Do not visualize it.");

                    AddChild(new Sprite(armRightTexture)
                    {
                        Position = new Vector2(13, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });

                    if (bodyEquipment != null)
                    {
                        if (bodyEquipment.Scheme.Sid == "traveler")
                        {
                            AddChild(new Sprite(_armRightTravelerTexture)
                            {
                                Position = new Vector2(13, -20),
                                Origin = new Vector2(0.5f, 0.5f)
                            });
                        }
                    }
                }
            }
            else
            {
                AddChild(new Sprite(armRightTexture)
                {
                    Position = new Vector2(13, -20),
                    Origin = new Vector2(0.5f, 0.5f)
                });

                if (bodyEquipment != null)
                {
                    if (bodyEquipment.Scheme.Sid == "traveler")
                    {
                        AddChild(new Sprite(_armRightTravelerTexture)
                        {
                            Position = new Vector2(13, -20),
                            Origin = new Vector2(0.5f, 0.5f)
                        });
                    }
                }
            }
        }

        private void EquipmentModule_EquipmentChanged(object? sender, EquipmentChangedEventArgs e)
        {
            var childrenSprites = GetChildren().ToArray();
            foreach (var child in childrenSprites)
            {
                RemoveChild(child);
            }

            CreateSpriteHierarchy(_equipmentModule);
        }

        public SpriteContainer RootSprite => this;
        public Vector2 HitEffectPosition => Vector2.UnitY * -24;
    }
}