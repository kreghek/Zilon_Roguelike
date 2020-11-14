using Zilon.Core.Schemes;

// ReSharper disable All
// Убрать отключение инспекций после разработки крафта

namespace Zilon.Core.Props
{
    /// <summary>
    ///     Концепт предмета.
    /// </summary>
    /// <remarks>
    ///     Является предметом, который можно изучить любым персонажем,
    ///     если он соответствует условиям концепта.
    ///     Концепт (рецепт) является концептуальной схемой предмета (черновик, чертёж).
    ///     После прототипирования концепта персонаж изучает результат и уже точно знает рецепт
    ///     изготовления предмета.
    /// </remarks>
    public sealed class Concept : PropBase
    {
        /// <summary>
        ///     Конструктор.
        /// </summary>
        /// <param name="scheme"> Схема концепта. Сейчас всегда одна схема - conceptual-scheme. </param>
        /// <param name="conceptProp"> Схема предмета, описанного в концепте. </param>
        [ExcludeFromCodeCoverage]
        public Concept(IPropScheme scheme, IPropScheme conceptProp) : base(scheme)
        {
            ConceptProp = conceptProp;
        }

        /// <summary>
        ///     Предмет, описанный в концептуальной схеме.
        /// </summary>
        public IPropScheme ConceptProp { get; }
    }
}