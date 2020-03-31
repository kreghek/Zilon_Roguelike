using System;
using System.Collections.Generic;

using Zilon.Core.Localization;

namespace Zilon.Core.Persons
{
    public class Disease : IDisease
    {
        public DiseaseName Name { get; }

        public Disease(DiseaseName name)
        {
            Name = name;
        }
    }

    public struct DiseaseName : IEquatable<DiseaseName>
    {
        public DiseaseName(ILocalizedString primary, ILocalizedString primaryPrefix, ILocalizedString secondary)
        {
            Primary = primary ?? throw new ArgumentNullException(nameof(primary));
            PrimaryPrefix = primaryPrefix;
            Secondary = secondary;
        }

        public ILocalizedString Primary { get; }
        
        public ILocalizedString PrimaryPrefix { get; set; }

        public ILocalizedString Secondary { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DiseaseName name && Equals(name);
        }

        public override int GetHashCode()
        {
            var hashCode = -2010629649;
            hashCode = hashCode * -1521134295 + EqualityComparer<ILocalizedString>.Default.GetHashCode(Primary);
            hashCode = hashCode * -1521134295 + EqualityComparer<ILocalizedString>.Default.GetHashCode(PrimaryPrefix);
            hashCode = hashCode * -1521134295 + EqualityComparer<ILocalizedString>.Default.GetHashCode(Secondary);
            return hashCode;
        }

        public static bool operator ==(DiseaseName left, DiseaseName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiseaseName left, DiseaseName right)
        {
            return !(left == right);
        }

        public bool Equals(DiseaseName other)
        {
            return EqualityComparer<ILocalizedString>.Default.Equals(Primary, other.Primary) &&
                   EqualityComparer<ILocalizedString>.Default.Equals(PrimaryPrefix, other.PrimaryPrefix) &&
                   EqualityComparer<ILocalizedString>.Default.Equals(Secondary, other.Secondary);
        }
    }
}
