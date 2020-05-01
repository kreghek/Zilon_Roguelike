using Zilon.Core.PersonModules;

namespace Zilon.Core.Persons
{
    public interface ISurvivalStatEffect
    {
        void Apply(ISurvivalModule survivalData);
    }
}
