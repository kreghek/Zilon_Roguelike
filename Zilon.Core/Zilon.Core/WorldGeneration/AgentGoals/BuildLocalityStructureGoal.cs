namespace Zilon.Core.WorldGeneration.AgentGoals
{
    public class BuildLocalityStructureGoal: ReGoapGoalBase<string, object>
    {

        public BuildLocalityStructureGoal(): base()
        {
            goal.Set("buildStructure", true);
        }
    }
}
