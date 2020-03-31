using Zilon.Core.Localization;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс болезни, поразившей персонажа.
    /// </summary>
    public interface IDisease
    {
        DiseaseName Name { get; }
    }
}
