namespace Zilon.Core.Tactics.Events
{
    public interface ITacticEvent
    {
        string Id { get; }
        string TriggerName { get; }
        TargetTriggerGroup[] Targets { get; }
    }
}
