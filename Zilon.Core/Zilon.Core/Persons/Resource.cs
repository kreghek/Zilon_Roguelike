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

        /// <summary>
        /// Разделяет текущий сток ресурсов и формирует новый экземпляр с указанным количеством.
        /// </summary>
        /// <param name="count"> Коичество единиц ресурса в новой куче. </param>
        /// <returns> Экземпляр отделённой кучи ресурсов. </returns>
        public Resource CreateHeap(int count)
        {
            var resource2 = new Resource(Scheme, count);
            Count -= count;
            return resource2;
        }
    }
}
