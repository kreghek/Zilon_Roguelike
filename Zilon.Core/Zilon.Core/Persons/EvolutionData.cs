namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных по развитию персонажа.
    /// </summary>
    public class EvolutionData : IEvolutionData
    {
        public EvolutionData()
        {
            ActivePerks = new IPerk[0];
            ArchievedPerks = new IPerk[0];
        }

        public IPerk[] ActivePerks { get; }

        public IPerk[] ArchievedPerks { get; }
    }
}
