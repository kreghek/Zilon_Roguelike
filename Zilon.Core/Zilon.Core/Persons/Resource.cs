using System;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class Resource : PropBase
    {
        public Resource(PropScheme scheme, int count) : base(scheme)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Ресурсов не может быть 0 или меньше.", nameof(count));
            }

            Count = count;
        }

        /// <summary>
        /// Количество единиц ресурса.
        /// </summary>
        public int Count { get; set; }
    }
}
