using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class HumanoidGraphics : SpriteContainer, IActorGraphics
    {
        private readonly IEquipmentModule _equipmentModule;

        private readonly IPersonVisualizationContentStorage _personVisualizationContentStorage;

        public HumanoidGraphics(IEquipmentModule equipmentModule,
            IPersonVisualizationContentStorage personVisualizationContentStorage)
        {
            _equipmentModule = equipmentModule;

            _personVisualizationContentStorage = personVisualizationContentStorage;

            CreateSpriteHierarchy(equipmentModule);

            equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
        }

        private void CreateSpriteHierarchy(IEquipmentModule equipmentModule)
        {
            DrawLeftHand(equipmentModule);
            var humanParts = DrawLegs();

            DrawChest(equipmentModule);

            var headTexture = humanParts.Single(x => x.Type == BodyPartType.Head).Texture;
            AddChild(new Sprite(headTexture)
            {
                Position = new Vector2(-0, -20),
                Origin = new Vector2(0.5f, 1)
            });

            DrawRightHand(equipmentModule);
        }

        private IEnumerable<BodyPart> DrawLegs()
        {
            var humanParts = _personVisualizationContentStorage.GetHumanParts();
            var legsTexture = humanParts.Single(x => x.Type == BodyPartType.LegsIdle).Texture;

            AddChild(new Sprite(legsTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.75f)
            });

            var equipmentBody = _equipmentModule[1];
            if (equipmentBody != null)
            {
                var equipedBodyParts = _personVisualizationContentStorage.GetBodyParts(equipmentBody.Scheme.Sid);
                var equipedLegPart = equipedBodyParts.SingleOrDefault(x => x.Type == BodyPartType.LegsIdle);
                if (equipedLegPart != null)
                {
                    AddChild(new Sprite(equipedLegPart.Texture)
                    {
                        Position = new Vector2(0, 0),
                        Origin = new Vector2(0.5f, 0.75f)
                    });
                }
            }

            return humanParts;
        }

        private void DrawChest(IEquipmentModule equipmentModule)
        {
            var humanParts = _personVisualizationContentStorage.GetHumanParts();
            var chestTexture = humanParts.Single(x => x.Type == BodyPartType.Chest).Texture;

            AddChild(new Sprite(chestTexture)
            {
                Position = new Vector2(3, -22),
                Origin = new Vector2(0.5f, 0.5f)
            });

            // This slot is body.
            var equipment = equipmentModule[1];
            if (equipment != null)
            {
                var equipmentChestParts = _personVisualizationContentStorage.GetBodyParts(equipment.Scheme.Sid);
                var equipmentChestTexture = equipmentChestParts.Single(x => x.Type == BodyPartType.Chest).Texture;

                AddChild(new Sprite(equipmentChestTexture)
                {
                    Position = new Vector2(3, -22),
                    Origin = new Vector2(0.5f, 0.5f)
                });
            }
        }

        private void DrawLeftHand(IEquipmentModule equipmentModule)
        {
            var humanParts = _personVisualizationContentStorage.GetHumanParts();
            var armLeftTexture = humanParts.Single(x => x.Type == BodyPartType.ArmLeft).Texture;

            // Slot 1 according the person scheme is body.
            var bodyEquipment = equipmentModule[1];
            BodyPart? equipedLeftHandPart = null;
            if (bodyEquipment != null)
            {
                var equipmentParts = _personVisualizationContentStorage.GetBodyParts(bodyEquipment.Scheme.Sid);
                equipedLeftHandPart = equipmentParts.SingleOrDefault(x => x.Type == BodyPartType.ArmLeft);
            }

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

                    if (equipedLeftHandPart != null)
                    {
                        AddChild(new Sprite(equipedLeftHandPart.Texture)
                        {
                            Position = new Vector2(-10, -20),
                            Origin = new Vector2(0.5f, 0.5f)
                        });
                    }
                }
                else if (equipmentTags.Contains(PropTags.Equipment.Weapon))
                {
                    AddChild(new Sprite(armLeftTexture)
                    {
                        Position = new Vector2(-10, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });

                    if (equipedLeftHandPart != null)
                    {
                        AddChild(new Sprite(equipedLeftHandPart.Texture)
                        {
                            Position = new Vector2(-10, -20),
                            Origin = new Vector2(0.5f, 0.5f)
                        });
                    }

                    var handParts = _personVisualizationContentStorage.GetHandParts(equipment.Scheme.Sid);
                    var weaponBaseTexture = handParts.Single(x => x.Type == HandPartType.Base).Texture;

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

                    var handParts = _personVisualizationContentStorage.GetHandParts(equipment.Scheme.Sid);
                    var shieldBackBaseTexture = handParts.Single(x => x.Type == HandPartType.BaseBack).Texture;

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

                    if (equipedLeftHandPart != null)
                    {
                        AddChild(new Sprite(equipedLeftHandPart.Texture)
                        {
                            Position = new Vector2(-10, -20),
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

                    if (equipedLeftHandPart != null)
                    {
                        AddChild(new Sprite(equipedLeftHandPart.Texture)
                        {
                            Position = new Vector2(-10, -20),
                            Origin = new Vector2(0.5f, 0.5f)
                        });
                    }
                }
            }
            else
            {
                AddChild(new Sprite(armLeftTexture)
                {
                    Position = new Vector2(13, -20),
                    Origin = new Vector2(0.5f, 0.5f)
                });

                if (equipedLeftHandPart != null)
                {
                    AddChild(new Sprite(equipedLeftHandPart.Texture)
                    {
                        Position = new Vector2(-10, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
            }
        }

        private void DrawRightHand(IEquipmentModule equipmentModule)
        {
            var humanParts = _personVisualizationContentStorage.GetHumanParts();
            var armRightTexture = humanParts.Single(x => x.Type == BodyPartType.ArmRightSimple).Texture;

            // Slot 3 according the person scheme is second/left hand.
            var weaponEquipment = equipmentModule[3];
            // Slot 1 according the person scheme is body.
            var bodyEquipment = equipmentModule[1];

            BodyPart? equipedRightHandPart = null;
            if (bodyEquipment != null)
            {
                var equipmentParts = _personVisualizationContentStorage.GetBodyParts(bodyEquipment.Scheme.Sid);
                equipedRightHandPart = equipmentParts.SingleOrDefault(x => x.Type == BodyPartType.ArmRightSimple);
            }

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

                    if (equipedRightHandPart != null)
                    {
                        AddChild(new Sprite(equipedRightHandPart.Texture)
                        {
                            Position = new Vector2(13, -20),
                            Origin = new Vector2(0.5f, 0.5f)
                        });
                    }
                }
                else if (equipmentTags.Contains(PropTags.Equipment.Weapon))
                {
                    var handParts = _personVisualizationContentStorage.GetHandParts(weaponEquipment.Scheme.Sid);
                    var weaponBaseTexture = handParts.Single(x => x.Type == HandPartType.Base).Texture;

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

                    if (equipedRightHandPart != null)
                    {
                        AddChild(new Sprite(equipedRightHandPart.Texture)
                        {
                            Position = new Vector2(13, -20),
                            Origin = new Vector2(0.5f, 0.5f)
                        });
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

                    if (equipedRightHandPart != null)
                    {
                        AddChild(new Sprite(equipedRightHandPart.Texture)
                        {
                            Position = new Vector2(13, -20),
                            Origin = new Vector2(0.5f, 0.5f)
                        });
                    }

                    var handParts = _personVisualizationContentStorage.GetHandParts(weaponEquipment.Scheme.Sid);
                    var shieldBaseTexture = handParts.Single(x => x.Type == HandPartType.Base).Texture;

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

                    if (equipedRightHandPart != null)
                    {
                        AddChild(new Sprite(equipedRightHandPart.Texture)
                        {
                            Position = new Vector2(13, -20),
                            Origin = new Vector2(0.5f, 0.5f)
                        });
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

                if (equipedRightHandPart != null)
                {
                    AddChild(new Sprite(equipedRightHandPart.Texture)
                    {
                        Position = new Vector2(13, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
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