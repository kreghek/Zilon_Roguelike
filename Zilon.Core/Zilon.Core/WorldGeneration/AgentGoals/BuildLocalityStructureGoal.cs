using Zilon.Core.WorldGeneration.LocalityStructures;

namespace Zilon.Core.WorldGeneration.AgentGoals
{
    public class BuildLocalityStructureGoal: ReGoapGoalBase<string, object>
    {

        public BuildLocalityStructureGoal(Locality locality, ILocalityStructure localityStructure): base()
        {
            goal.Set($"structure_{localityStructure.Name}_in_{locality.Name}", true);

            Name = $"Build {localityStructure.Name} IN {locality.Name}";
        }
    }
}
