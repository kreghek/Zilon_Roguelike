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

        private void DrawChest(IEquipmentModule equipmentModule)
        {
            var humanParts = _personVisualizationContentStorage.GetHumanParts();
            var chestTexture = humanParts.Single(x => x.Type == BodyPartType.Chest).Texture;

            AddChild(CreateChestSprite(chestTexture));

            // This slot is body.
            var bodyEquipment = equipmentModule[1];
            if (bodyEquipment != null)
            {
                var bodyEquipmentSid = bodyEquipment.Scheme.Sid;
                if (bodyEquipmentSid != null)
                {
                    var equipmentChestParts = _personVisualizationContentStorage.GetBodyParts(bodyEquipmentSid);
                    var equipmentChestTexture = equipmentChestParts.Single(x => x.Type == BodyPartType.Chest).Texture;

                    AddChild(CreateChestSprite(equipmentChestTexture));
                }
                else
                {
                    Debug.Fail("There are no schemes without SID. Looks like some kind of error.");
                }
            }
        }

        private static Sprite CreateChestSprite(Microsoft.Xna.Framework.Graphics.Texture2D chestTexture)
        {
            return new Sprite(chestTexture)
            {
                Position = new Vector2(3, -22),
                Origin = new Vector2(0.5f, 0.5f)
            };
        }

        private void DrawLeftHand(IEquipmentModule equipmentModule)
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

                    AddLeftArmHierarchy();
                }
                else if (equipmentTags.Contains(PropTags.Equipment.Weapon))
                {
                    AddLeftArmHierarchy();

                    AddLeftWeaponHierarchy(equipment);
                }
                else if (equipmentTags.Contains(PropTags.Equipment.Shield))
                {
                    // For a shield in this hand draw shield back sprite first.
                    // So it will looks like the person bear shield by inner side.

                    AddLeftShieldHierarchy(equipment);

                    AddLeftArmHierarchy();
                }
                else
                {
                    Debug.Fail("Unknown tag in the equipment.");

                    AddLeftArmHierarchy();
                }
            }
            else
            {
                AddLeftArmHierarchy();
            }
        }

        private void AddLeftShieldHierarchy(Zilon.Core.Props.Equipment equipment)
        {
            var shieldSid = equipment.Scheme.Sid;
            if (shieldSid == null)
            {
                Debug.Fail("There are no scheme without SID. This looks loke some kind of error.");
                // Do nothing. We can't draw unknown shield.
                return;
            }

            var handParts = _personVisualizationContentStorage.GetHandParts(shieldSid);
            var shieldBackBaseTexture = handParts.Single(x => x.Type == HandPartType.BaseBack).Texture;

            AddChild(new Sprite(shieldBackBaseTexture)
            {
                Position = new Vector2(-14, -21),
                Origin = new Vector2(0.5f, 0.5f)
            });
        }

        private void AddLeftWeaponHierarchy(Zilon.Core.Props.Equipment equipment)
        {
            var weaponSid = equipment.Scheme.Sid;
            if (weaponSid == null)
            {
                // Do nothing. We can't draw unknown weapon.
                Debug.Fail("There are no schemes without SID. Looks like some kind of error.");
            }

            var handParts = _personVisualizationContentStorage.GetHandParts(weaponSid);
            var weaponBaseTexture = handParts.Single(x => x.Type == HandPartType.Base).Texture;

            AddChild(new Sprite(weaponBaseTexture)
            {
                Position = new Vector2(-14, -21),
                Origin = new Vector2(0.5f, 0.5f)
            });
        }

        private void AddLeftArmHierarchy()
        {
            var humanParts = _personVisualizationContentStorage.GetHumanParts();
            var armLeftTexture = humanParts.Single(x => x.Type == BodyPartType.ArmLeft).Texture;

            // Slot 1 according the person scheme is body.
            var dressedLeftArmPart = GetDressedPartAccordingBodySlot(_equipmentModule, BodyPartType.ArmLeft);

            AddChild(new Sprite(armLeftTexture)
            {
                Position = new Vector2(-10, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });

            if (dressedLeftArmPart != null)
            {
                AddChild(new Sprite(dressedLeftArmPart.Texture)
                {
                    Position = new Vector2(-10, -20),
                    Origin = new Vector2(0.5f, 0.5f)
                });
            }
        }

        private BodyPart? GetDressedPartAccordingBodySlot(IEquipmentModule equipmentModule, BodyPartType partType)
        {
            // Slot 1 according the person scheme is body.
            var bodyEquipment = equipmentModule[1];
            if (bodyEquipment == null)
            {
                return null;
            }

            var bodyEquipmentSid = bodyEquipment.Scheme.Sid;
            if (bodyEquipmentSid != null)
            {
                var equipmentParts = _personVisualizationContentStorage.GetBodyParts(bodyEquipmentSid);
                return equipmentParts.SingleOrDefault(x => x.Type == partType);
            }
            else
            {
                Debug.Fail("There are no schemes without SID. So this looks like some kind of error.");
                return null;
            }
        }

        private IEnumerable<BodyPart> DrawLegs()
        {
            var humanParts = _personVisualizationContentStorage.GetHumanParts();
            var legsTexture = humanParts.Single(x => x.Type == BodyPartType.LegsIdle).Texture;

            AddChild(CreateLegsSprite(legsTexture));

            var equipmentBody = _equipmentModule[1];
            if (equipmentBody != null)
            {
                var bodyEquipmentSid = equipmentBody.Scheme.Sid;
                if (bodyEquipmentSid != null)
                {
                    var dressedBodyParts = _personVisualizationContentStorage.GetBodyParts(bodyEquipmentSid);
                    var equipedLegPart = dressedBodyParts.SingleOrDefault(x => x.Type == BodyPartType.LegsIdle);
                    if (equipedLegPart != null)
                    {
                        AddChild(CreateLegsSprite(equipedLegPart.Texture));
                    }
                }
                else
                {
                    Debug.Fail("There are no schemes without SID. So this looks like some kind of error.");
                }
            }

            return humanParts;
        }

        private static Sprite CreateLegsSprite(Microsoft.Xna.Framework.Graphics.Texture2D legsTexture)
        {
            return new Sprite(legsTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.75f)
            };
        }

        private void DrawRightHand(IEquipmentModule equipmentModule)
        {
            // Slot 3 according the person scheme is second/left hand.
            var weaponEquipment = equipmentModule[3];
            if (weaponEquipment == null)
            {
                // There is nothing in right hand.
                // This is normal situation. So just draw arm unequiped.

                AddRightArmHierarchy();

                return;
            }

            var equipmentTags = weaponEquipment.Scheme.Tags;
            if (equipmentTags is null)
            {
                // Now a person can equiped only weapons or tools.
                // All weapons or tools has corresponding tags.
                Debug.Fail("There is no scenario then equipment has no tags.");

                AddRightArmHierarchy();
            }
            else if (equipmentTags.Contains(PropTags.Equipment.Weapon))
            {
                AddRightWeaponHierarchy(weaponEquipment);

                AddRightArmHierarchy();
            }
            else if (equipmentTags.Contains(PropTags.Equipment.Shield))
            {
                // For shield draw arm first because the base of the sheild
                // should be cover the arm.

                AddRightArmHierarchy();

                AddRightShieldHierarchy(weaponEquipment);
            }
            else
            {
                Debug.Fail("Unknown tag of thing in hand. Do not visualize it.");

                AddRightArmHierarchy();
            }
        }

        private void AddRightShieldHierarchy(Zilon.Core.Props.Equipment equipment)
        {
            var shieldSid = equipment.Scheme.Sid;
            if (shieldSid == null)
            {
                Debug.Fail("There are no schemes without SID. So this looks like some kind of error.");
                return;
            }

            var handParts = _personVisualizationContentStorage.GetHandParts(shieldSid);
            var shieldBaseTexture = handParts.Single(x => x.Type == HandPartType.Base).Texture;

            AddChild(new Sprite(shieldBaseTexture)
            {
                Position = new Vector2(11, -10),
                Origin = new Vector2(0.5f, 0.5f)
            });
        }

        private void AddRightWeaponHierarchy(Zilon.Core.Props.Equipment equipment)
        {
            var weaponSid = equipment.Scheme.Sid;
            if (weaponSid == null)
            {
                Debug.Fail("There are no schemes without SID. So this looks like some kind of error.");
                return;
            }

            var handParts = _personVisualizationContentStorage.GetHandParts(weaponSid);
            var weaponBaseTexture = handParts.Single(x => x.Type == HandPartType.Base).Texture;

            AddChild(new InvertedFlipXSprite(weaponBaseTexture)
            {
                Position = new Vector2(11, -10),
                Origin = new Vector2(0.5f, 0.5f),
                Rotation = (float)(-Math.PI / 2)
            });
        }

        private void AddRightArmHierarchy()
        {
            var humanParts = _personVisualizationContentStorage.GetHumanParts();
            var armRightTexture = humanParts.Single(x => x.Type == BodyPartType.ArmRightSimple).Texture;

            var dressedRightHandPart = GetDressedPartAccordingBodySlot(_equipmentModule, BodyPartType.ArmRightSimple);

            AddChild(CreateRightArmSprite(armRightTexture));

            if (dressedRightHandPart != null)
            {
                AddChild(CreateRightArmSprite(dressedRightHandPart.Texture));
            }
        }

        private static Sprite CreateRightArmSprite(Microsoft.Xna.Framework.Graphics.Texture2D armRightTexture)
        {
            return new Sprite(armRightTexture)
            {
                Position = new Vector2(13, -20),
                Origin = new Vector2(0.5f, 0.5f)
            };
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