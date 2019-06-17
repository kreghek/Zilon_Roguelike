namespace Zilon.Core.Persons
{
    public static class IPersonExtensions
    {
        public static bool CheckIsDead(this IPerson person)
        {
            if (person.Survival == null)
            {
                return false;
            }

            return person.Survival.IsDead;

        }
    }
}
