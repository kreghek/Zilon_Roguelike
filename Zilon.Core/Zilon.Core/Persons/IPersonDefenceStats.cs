using Zilon.Core.Components;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Характристики обороны персонажа против наступательных действий.
    /// </summary>
    public interface IPersonDefenceStats
    {
        PersonDefenceItem[] Defences { get; }
    }
}