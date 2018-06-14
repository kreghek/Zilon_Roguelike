using Zilon.Core.Persons;
using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tactics
{
    public class Actor
    {
        public Actor(Person person, MapNode node)
        {
            Person = person;
            Node = node;
        }

        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        public Person Person { get; }

        /// <summary>
        /// Текущий узел карты, в котором находится актёр.
        /// </summary>
        public MapNode Node { get; set; }
    }
}
