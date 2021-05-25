using System;
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

            // Slot 2 according the person scheme is second/left hand.
            var equipment2 = equipmentModule[2];
            if (equipment2 != null)
            {
                if (equipment2.Scheme.Tags.Contains(PropTags.Equipment.Weapon))
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
                else if (equipment2.Scheme.Tags.Contains(PropTags.Equipment.Shield))
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
            }
        }

        public SpriteContainer RootSprite => this;
        public Vector2 HitEffectPosition => Vector2.UnitY * -24;
    }
}