using System.Drawing;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Государство.
    /// </summary>
    public class Realm
    {
        public string Name { get; set; }
        public Color Color;

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
