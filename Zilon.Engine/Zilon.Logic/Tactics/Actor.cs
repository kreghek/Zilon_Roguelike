using Zilon.Logic.Persons;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Tactics
{
    public class Actor
    {
        private readonly Person person;
        private MapNode currentNode;

        public Actor(Person person, MapNode currentNode)
        {
            this.person = person;
            this.currentNode = currentNode;
        }

        public Person Person => person;

        public MapNode CurrentNode { get => currentNode; set => currentNode = value; }


    }
}
