namespace Zilon.Core.Persons
{
    /// <summary>
    /// Объект, представляющий болезнь в игре.
    /// </summary>
    public class Disease : IDisease
    {
        /// <summary>
        /// Наименование болезни.
        /// </summary>
        public DiseaseName Name { get; }

        public Disease(DiseaseName name)
        {
            Name = name;
        }
    }
}
