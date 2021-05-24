using Zilon.Core.PersonModules;

namespace Zilon.Core.Persons
{
    public interface ISurvivalStatCondition
    {
        void Apply(ISurvivalModule survivalData);
    }
}