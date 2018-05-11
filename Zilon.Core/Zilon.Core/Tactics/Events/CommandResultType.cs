namespace Zilon.Core.Tactics.Events
{
    public enum CommandResultType
    {
        Complete,
        InnerError,
        InvalidContext,
        NotOwner,
        NotEnoughMP,
        NotEnoughAP,
        NoFreeSpace,
        OutOfRange,
        NotAimed,
        MaxAimValue,
        NotTimeOut,
        PathNotFound
    }
}
