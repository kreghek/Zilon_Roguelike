﻿using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Props
{
    public sealed class Resource : PropBase
    {
        private int _count;

        [ExcludeFromCodeCoverage]
        public Resource(IPropScheme scheme, int count) : base(scheme)
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
        public int Count
        {
            get => _count;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Величина не может быть меньше или равна 0.");
                }

                _count = value;
                DoChange();
            }
        }

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

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{Scheme} x {Count}";
        }

        [ExcludeFromCodeCoverage]
        private void DoChange()
        {
            Changed?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs>? Changed;
    }
}