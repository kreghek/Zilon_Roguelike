using Zilon.Core.PersonModules;

namespace Zilon.Core.Persons
{
    public static class IPersonExtensions
    {
        public static bool CheckIsDead(this IPerson person)
        {
            ISurvivalModule survivalModule = person.GetModuleSafe<ISurvivalModule>();
            if (survivalModule is null)
            {
                // Те, у кого нет модуля выживания, не могут умереть.
                return false;
            }

            return survivalModule.IsDead;
        }
    }
}