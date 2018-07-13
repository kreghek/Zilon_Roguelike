using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class Resource : PropBase
    {
        public Resource(PropScheme scheme) : base(scheme)
        {
        }

        /// <summary>
        /// Количество единиц ресурса.
        /// </summary>
        public int Count { get; set; }
    }
}
