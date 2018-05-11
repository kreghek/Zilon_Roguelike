namespace Zilon.Logic.Tactics.Events
{
    public interface ICommandEvent
    {
        string Id { get; }
        string TriggerName { get; }
        TargetTriggerGroup[] Targets { get; }
    }
}
