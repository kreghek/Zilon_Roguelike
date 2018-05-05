using Zilon.Logic.Math;
using Zilon.Logic.Persons;

namespace Zilon.Logic.Tactics
{
    public class Actor
    {
        private readonly Person person;

        public Actor(Person person)
        {
            this.person = person;
        }

        public Person Person => person;
        public Vector2 Position { get; set; }
    }
}
