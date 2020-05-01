using Zilon.Core.PersonModules;

namespace Zilon.Core.Persons
{
    public static class IPersonExtensions
    {
        public static bool CheckIsDead(this IPerson person)
        {
            if (person.GetModuleSafe<ISurvivalModule>() is null)
            {
                return false;
            }

            return person.GetModule<ISurvivalModule>().IsDead;

        }
    }
}
