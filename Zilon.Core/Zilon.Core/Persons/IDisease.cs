using Zilon.Core.Localization;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс болезни, поразившей персонажа.
    /// </summary>
    public interface IDisease
    {
        ILocalizedString Primary { get; }
        ILocalizedString PrimaryPrefix { get; }
    }
}
