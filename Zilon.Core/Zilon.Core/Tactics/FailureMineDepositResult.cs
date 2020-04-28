namespace Zilon.Core.Tactics
{
    public sealed class FailureMineDepositResult : IMineDepositResult
    {
        public bool Success { get => false; }
    }
}
