namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Состояние узла карты сектора.
    /// </summary>
    public enum SectorMapNodeFowState
    {
        /// <summary>
        /// Узел не исследован. Персонажу не известно о его существовании.
        /// </summary>
        TerraIncognita,

        /// <summary>
        /// Узел графа был открыт, но сейчас не наблюдается.
        /// </summary>
        Explored,

        /// <summary>
        /// Узел графа наблюдается персонажем.
        /// </summary>
        Observing
    }
}