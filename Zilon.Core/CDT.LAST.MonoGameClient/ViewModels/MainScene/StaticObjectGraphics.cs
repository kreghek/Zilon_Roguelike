using System.Diagnostics;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Tactics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class StaticObjectGraphics : SpriteContainer
    {
        public StaticObjectGraphics(Game game, IStaticObject staticObject)
        {
            var textureName = GetTextureNameByPurpose(staticObject.Purpose);
            var staticObjectTexture = game.Content.Load<Texture2D>($"Sprites/game-objects/Environment/{textureName}");
            var staticObjectSprite = new Sprite(staticObjectTexture)
            {
                Origin = new Vector2(0.5f, 0.75f)
            };

            AddChild(staticObjectSprite);
        }

        private static string GetTextureNameByPurpose(PropContainerPurpose purpose)
        {
            switch (purpose)
            {
                case PropContainerPurpose.CherryBrush:
                case PropContainerPurpose.Treasures:
                    return "Grass";

                case PropContainerPurpose.OreDeposits:
                    return "StoneBrown";

                case PropContainerPurpose.StoneDeposits:
                    return "StoneBlue";

                case PropContainerPurpose.TrashHeap:
                case PropContainerPurpose.Trash:
                    return "TrashHeap";

                case PropContainerPurpose.Puddle:
                    return "Puddle";

                case PropContainerPurpose.Pit:
                    return "Pit";

                case PropContainerPurpose.Loot:
                    return "LootBag";

                default:
                    Debug.Fail($"Texture for specified purpose {purpose}.");
                    return "Grass";
            }
        }
    }
}