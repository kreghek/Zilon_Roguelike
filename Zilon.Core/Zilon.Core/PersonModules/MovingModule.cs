using System;

using Zilon.Core.World;

namespace Zilon.Core.PersonModules
{
    public class MovingModule : IMovingModule
    {
        private int BASE_COST = GlobeMetrics.OneIterationLength;
        private const int BASE_DEXTERITY = 10;
        private const int COST_PER_DEXTERITY_UNIT = 1;

        private readonly IAttributesModule _attributesModule;

        public MovingModule(IAttributesModule attributesModule)
        {
            _attributesModule = attributesModule;
        }

        public string Key => nameof(IMovingModule);
        public bool IsActive { get; set; }

        public int CalculateCost()
        {
            var dexterityAttribute = _attributesModule.GetAttribute(PersonAttributeType.Dexterity);
            int significantValue = (int)Math.Ceiling(dexterityAttribute.Value);
            var diffValue = significantValue - BASE_DEXTERITY;
            return BASE_COST - (diffValue * COST_PER_DEXTERITY_UNIT);
        }
    }
}