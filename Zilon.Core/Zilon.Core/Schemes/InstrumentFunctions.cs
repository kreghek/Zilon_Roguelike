namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Функиональное назначение инструментов.
    /// </summary>
    [Flags]
    public enum InstrumentFunctions
    {
        StrongAssembly = 1 << 1,
        StrongWoodWork = 1 << 2,
        StrongMetalWork = 1 << 3,
        StrongChimestry = 1 << 4
    }
}