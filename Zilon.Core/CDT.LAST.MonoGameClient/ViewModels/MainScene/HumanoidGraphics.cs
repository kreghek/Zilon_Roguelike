using System;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Common;
using Zilon.Core.PersonModules;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class HumanoidGraphics : SpriteContainer, IActorGraphics
    {
        private readonly IEquipmentModule _equipmentModule;

        public HumanoidGraphics(IEquipmentModule equipmentModule, ContentManager contentManager)
        {
            _equipmentModule = equipmentModule;

            var headTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/head");
            var bodyTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/body");
            var legsTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/legs-idle");
            var armLeftTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/arm-left-simple");
            var armRightTexture = contentManager.Load<Texture2D>("Sprites/game-objects/human/arm-right-simple");

            var weaponBaseTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/ShortSwordBase");
            var shieldBaseTexture = contentManager.Load<Texture2D>("Sprites/game-objects/Equipments/WoodenShieldBase");

            //TODO Handle events of changing equipments.

            DrawLeftHand(equipmentModule, armLeftTexture, weaponBaseTexture, shieldBaseTexture);

            AddChild(new Sprite(legsTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.75f)
            });

            AddChild(new Sprite(bodyTexture)
            {
                Position = new Vector2(3, -22),
                Origin = new Vector2(0.5f, 0.5f)
            });

            AddChild(new Sprite(headTexture)
            {
                Position = new Vector2(-0, -20),
                Origin = new Vector2(0.5f, 1)
            });

            DrawRightHand(equipmentModule, armRightTexture, weaponBaseTexture, shieldBaseTexture);
        }

        private void DrawLeftHand(IEquipmentModule equipmentModule, Texture2D armLeftTexture, Texture2D weaponBaseTexture, Texture2D shieldBackBaseTexture)
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

        private void DrawRightHand(IEquipmentModule equipmentModule, Texture2D armRightTexture, Texture2D weaponBaseTexture, Texture2D shieldBaseTexture)
        {
            // Slot 3 according the person scheme is second/left hand.
            var equipment = equipmentModule[3];
            if (equipment != null)
            {
                var equipmentTags = equipment.Scheme.Tags;
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

                    AddChild(new Sprite(shieldBaseTexture)
                    {
                        Position = new Vector2(11, -10),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
                else
                {
                    AddChild(new Sprite(armRightTexture)
                    {
                        Position = new Vector2(13, -20),
                        Origin = new Vector2(0.5f, 0.5f)
                    });
                }
            }
            else
            {
                AddChild(new Sprite(armRightTexture)
                {
                    Position = new Vector2(13, -20),
                    Origin = new Vector2(0.5f, 0.5f)
                });
            }
        }

        public SpriteContainer RootSprite => this;
        public Vector2 HitEffectPosition => Vector2.UnitY * -24;
    }
}