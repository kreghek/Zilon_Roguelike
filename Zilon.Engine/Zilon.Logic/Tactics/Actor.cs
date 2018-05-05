using Zilon.Logic.Persons;
using Zilon.Logic.Tactics.Map;

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
    }
}
