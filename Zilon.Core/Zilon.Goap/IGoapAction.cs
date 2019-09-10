namespace Zilon.Goap
{
    public interface IGoapAction
    {
        IEffects Effects { get; }

        IPreconditions Rpeconditions { get; }
    }
}
