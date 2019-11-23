namespace Zilon.Core.World
{
    public static class GlobeRegionPatterns
    {
        public static GlobeRegionPattern Angle
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

        public static GlobeRegionPattern Tringle
        {
            get
            {
                return new GlobeRegionPattern
                {
                    Values = new GlobeRegionPatternValue[,]{
                        { null, null, null, null, null, null },
                        { null, null, new GlobeRegionPatternValue(true), null, null, null },
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { null, new GlobeRegionPatternValue(true), null, null, new GlobeRegionPatternValue(true), null },
                        { null, null, null, null, null, null },
                    }
                };
            }
        }

        public static GlobeRegionPattern Linear
        {
            get
            {
                return new GlobeRegionPattern
                {
                    Values = new GlobeRegionPatternValue[,]{
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { new GlobeRegionPatternValue(true), null, null, new GlobeRegionPatternValue(true), null, new GlobeRegionPatternValue(true) },
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                    }
                };
            }
        }

        public static GlobeRegionPattern Diagonal
        {
            get
            {
                return new GlobeRegionPattern
                {
                    Values = new GlobeRegionPatternValue[,]{
                        { new GlobeRegionPatternValue(true), null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { null, null, null, new GlobeRegionPatternValue(true), null, null },
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, new GlobeRegionPatternValue(true) },
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
                        { null, null, null, null, null, null },
                        { null, null, null, null, null, null },
                        { null, new GlobeRegionPatternValue(true), null, null, null, null },
                        { null, new GlobeRegionPatternValue(false){ IsStart = true }, new GlobeRegionPatternValue(true), null, null, null },
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
