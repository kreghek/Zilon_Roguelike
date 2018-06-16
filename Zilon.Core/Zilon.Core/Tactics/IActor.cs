using System;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public interface IActor
    {
        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        IPerson Person { get; }

        /// <summary>
        /// Текущий узел карты, в котором находится актёр.
        /// </summary>
        HexNode Node { get; }

        /// <summary>
        /// Перемещение актёра в указанный узел карты.
        /// </summary>
        /// <param name="targetNode"></param>
        void MoveToNode(HexNode targetNode);

        /// <summary>
        /// Происходит, когда актёр переместился.
        /// </summary>
        event EventHandler OnMoved;
    }
}