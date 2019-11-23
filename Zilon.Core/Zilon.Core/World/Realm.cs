using Zilon.Core.World;

namespace Zilon.Core.World
{
    /// <summary>
    /// Государство.
    /// </summary>
    public class Realm
    {
        public string Name { get; set; }

        public RealmBanner Banner { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
