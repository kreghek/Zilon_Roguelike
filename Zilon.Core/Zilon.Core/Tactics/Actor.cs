namespace Zilon.Core.Tactics
{
    using Zilon.Core.Persons;

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
