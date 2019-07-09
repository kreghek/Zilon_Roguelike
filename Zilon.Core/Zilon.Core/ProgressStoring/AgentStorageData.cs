namespace Zilon.Core.ProgressStoring
{
    public sealed class AgentStorageData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Текущее местоположение деятеля.
        /// </summary>
        public OffsetCoords Location { get; set; }

        /// <summary>
        /// Государтсво, на которое работает данный деятель.
        /// </summary>
        public string RealmId { get; set; }

        /// <summary>
        /// Текущие навыки агента.
        /// </summary>
        public AgentSkillStorageData[] Skills { get; set; }

        /// <summary>
        /// Условно, ХП деятеля. Его способность действовать в мире.
        /// </summary>
        /// <remarks>
        /// Когда ХП опускается до 0, деятель перестаёт существовать в мире, как деятель.
        /// </remarks>
        public int Hp { get; set; }
    }
}
