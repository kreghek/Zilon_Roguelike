using Zilon.Core.WorldGeneration.LocalityStructures;

namespace Zilon.Core.WorldGeneration.AgentGoals
{
    public class BuildLocalityStructureGoal: ReGoapGoalBase<string, object>
    {

        public BuildLocalityStructureGoal(Locality locality, BasicLocalityStructure localityStructure): base()
        {
            goal.Set($"buildStructure_{localityStructure.SpeciaName}_IN_{locality.Name}", true);
        }
    }
}
