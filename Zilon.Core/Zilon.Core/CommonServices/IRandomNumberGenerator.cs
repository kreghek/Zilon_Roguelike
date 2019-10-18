namespace Zilon.Core.CommonServices
{
    public interface IRandomNumberGenerator
    {
        double Next();
        void Reset();
    }
}