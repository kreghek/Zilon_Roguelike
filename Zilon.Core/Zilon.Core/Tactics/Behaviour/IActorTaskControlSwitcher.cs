namespace Zilon.Core.Tactics.Behaviour
{
    public interface IActorTaskControlSwitcher
    {
        ActorTaskSourceControl CurrentControl { get; }
        void Switch(ActorTaskSourceControl taskSourceControl);
    }
}