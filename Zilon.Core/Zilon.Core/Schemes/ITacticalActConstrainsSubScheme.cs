namespace Zilon.Core.Schemes
{
    public interface ITacticalActConstrainsSubScheme : ISubScheme
    {
        int? Cooldown { get; }
        int? UsageResource { get; }
        int? UsageResourceRegen { get; }
    }
}