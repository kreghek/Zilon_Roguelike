using System.Linq;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Xna.Framework;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization
{
    public sealed class HumanoidGraphics : SpriteContainer, IActorGraphics
    {
        private readonly IEquipmentModule _equipmentModule;
        private SpriteContainer? _outlined;

        private readonly IPersonVisualizationContentStorage _personVisualizationContentStorage;

        public HumanoidGraphics(IEquipmentModule equipmentModule,
            IPersonVisualizationContentStorage personVisualizationContentStorage)
        {
            _equipmentModule = equipmentModule;

            _personVisualizationContentStorage = personVisualizationContentStorage;

            CreateSpriteHierarchy(equipmentModule);

            equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
        }

        protected override void DoDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, float zindex)
        {
            if (_outlined is not null)
            {
                _outlined.Visible = ShowOutlined;
            }

            base.DoDraw(spriteBatch, zindex);
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

        private void CreateSpriteHierarchy(IEquipmentModule equipmentModule)
        {
            var outlinedHumanoidSprite = new HumanoidSprite(equipmentModule,
               _personVisualizationContentStorage,
               _personVisualizationContentStorage.GetHumanOutlinedParts());

            _outlined = outlinedHumanoidSprite;
            AddChild(_outlined);

            var mainHumanoidSprite = new HumanoidSprite(equipmentModule,
                _personVisualizationContentStorage,
                _personVisualizationContentStorage.GetHumanParts());
            AddChild(mainHumanoidSprite);
        }

        public SpriteContainer RootSprite => this;

        public Vector2 HitEffectPosition => Vector2.UnitY * -24;

        public bool ShowOutlined { get; set; }
    }
}