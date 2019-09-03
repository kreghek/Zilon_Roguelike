namespace Zilon.Core.WorldGeneration.AgentGoals
{
    public class BuildLocalityStructureGoal: ReGoapGoalBase<string, object>
    {
        protected override void Awake()
        {
            base.Awake();
            goal.Set("myRequirement", true);
        }
    }
}
