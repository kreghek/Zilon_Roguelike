namespace Zilon.Goap
{
    public interface IGoapAction
    {
        IGoapConnItem[] Effects { get; }

        IGoapConnItem[] Conditions { get; }
    }
}
