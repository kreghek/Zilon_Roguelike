namespace Zilon.Core.PersonModules
{
    public class MovingModule : IMovingModule
    {
        private const int BASE_COST = 1000;
        private const int BASE_DEXTERITY = 10;
        private const int COST_PER_DEXTERITY_UNIT = 25;

        private readonly IAttributesModule _attributesModule;

        public MovingModule(IAttributesModule attributesModule)
        {
            _attributesModule = attributesModule;
        }

        public string Key => nameof(IMovingModule);
        public bool IsActive { get; set; }

        public int CalculateCost()
        {
            PersonAttribute dexterityAttribute = _attributesModule.GetAttribute(PersonAttributeType.Dexterity);
            int significantValue = (int)Math.Ceiling(dexterityAttribute.Value);
            var diffValue = significantValue - BASE_DEXTERITY;
            return BASE_COST - (diffValue * COST_PER_DEXTERITY_UNIT);
        }
    }
}