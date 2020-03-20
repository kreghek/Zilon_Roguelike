namespace Zilon.Core.World
{
    public sealed class GlobeRegionDraftValue
    {
        public GlobeRegionDraftValue(GlobeRegionDraftValueType value)
        {
            Value = value;
        }

        public bool IsHome { get; set; }

        public bool IsStart { get; set; }

        public GlobeRegionDraftValueType Value { get; }
    }
}
