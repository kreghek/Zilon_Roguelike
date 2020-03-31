namespace Zilon.Core.Diseases
{
    /// <summary>
    /// Интерфейс болезни, поразившей персонажа.
    /// </summary>
    public interface IDisease
    {
        DiseaseName Name { get; }
    }
}
