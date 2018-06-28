namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема апдейта тактического действия.
    /// </summary>
    public class TacticalActUpdateSubScheme
    {
        /// <summary>
        /// Наименование.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Приоритет апдейта.
        /// </summary>
        /// <remarks>
        /// Если открыты условия между двумя и более апдейтов,
        /// то самый приоритетный будет перекрывать остальные
        /// </remarks>
        public int Priority { get; set; }

        /// <summary>
        /// Символьные идентфиикаторы перков, требуемые для открытия апдейта.
        /// </summary>
        public string[] RequiredPerkSids { get; set; }

        /// <summary>
        /// Требуемый уровень для открытия апдейта.
        /// </summary>
        public int? RequiredLevel { get; set; }
    }
}
