using System;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tactics
{
    public class Actor : IActor
    {
        public Actor(IPerson person, MapNode node)
        {
            Person = person;
            Node = node;
        }

        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        public IPerson Person { get; }

        /// <summary>
        /// Текущий узел карты, в котором находится актёр.
        /// </summary>
        public MapNode Node { get; set; }

        public event EventHandler OnMoved;

        public void MoveToNode(MapNode targetNode)
        {
            Node = targetNode;
            OnMoved?.Invoke(this, new EventArgs());
        }
    }
}
