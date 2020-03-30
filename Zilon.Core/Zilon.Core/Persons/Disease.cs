using System;
using Zilon.Core.Localization;

namespace Zilon.Core.Persons
{
    public class Disease : IDisease
    {
        public ILocalizedString Primary { get; }
        public ILocalizedString PrimaryPrefix { get; }

        public Disease(ILocalizedString primary, ILocalizedString primaryPrefix)
        {
            Primary = primary ?? throw new ArgumentNullException(nameof(primary));
            PrimaryPrefix = primaryPrefix;
        }
    }
}
