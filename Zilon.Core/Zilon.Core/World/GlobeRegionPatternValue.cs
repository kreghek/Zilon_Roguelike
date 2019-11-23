namespace Zilon.Core.World
{
    public sealed class GlobeRegionPatternValue
    {
        public GlobeRegionPatternValue(bool hasObject)
        {
            HasObject = hasObject;
        }

        public bool HasObject { get; }

        public bool IsHome { get; set; }

        public bool IsStart { get; set; }
    }
}
