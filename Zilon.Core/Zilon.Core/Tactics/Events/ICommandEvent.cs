namespace Zilon.Core.Tactics.Events
{
    public interface ICommandEvent
    {
        string Id { get; }
        string TriggerName { get; }
        TargetTriggerGroup[] Targets { get; }
    }
}
