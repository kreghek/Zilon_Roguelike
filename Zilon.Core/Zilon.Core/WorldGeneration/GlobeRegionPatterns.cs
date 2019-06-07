namespace Zilon.Core.WorldGeneration
{
    public static class GlobeRegionPatterns
    {
        public static GlobeRegionPattern Default
        {
            get
            {
                return new GlobeRegionPattern
                {
                    Values = new GlobeRegionPatternValue[,]{
                        { null, null, null, null, null, null },
                        { null, null, null, null, new GlobeRegionPatternValue(true), null },
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { null, new GlobeRegionPatternValue(true), null, null, new GlobeRegionPatternValue(true), null },
                        { null, null, null, null, null, null },
                    }
                };
            }
        }

        public static GlobeRegionPattern Start
        {
            get
            {
                return new GlobeRegionPattern
                {
                    Values = new GlobeRegionPatternValue[,]{
                        { null, null, null, null, null, null },
                        { null, null, null, null, new GlobeRegionPatternValue(true), null },
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { null, new GlobeRegionPatternValue(true){ IsStart = true }, null, null, new GlobeRegionPatternValue(true), null },
                        { null, null, null, null, null, null },
                    }
                };
            }
        }

        public static GlobeRegionPattern Home
        {
            get
            {
                return new GlobeRegionPattern
                {
                    Values = new GlobeRegionPatternValue[,]{
                        { null, null, null, null, null, null },
                        { null, null, null, null, new GlobeRegionPatternValue(true), null },
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { null, new GlobeRegionPatternValue(true){ IsHome = true }, null, null, new GlobeRegionPatternValue(true), null },
                        { null, null, null, null, null, null },
                    }
                };
            }
        }
    }
}
