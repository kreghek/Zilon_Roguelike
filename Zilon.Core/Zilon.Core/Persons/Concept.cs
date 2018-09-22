using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Концепт предмета.
    /// </summary>
    /// <remarks>
    /// Является предметом, который можно изучить любым персонажем,
    /// если он соответствует условиям концепта.
    /// 
    /// Концепт (рецепт) является концептуальной схемой предмета (черновик, чертёж).
    /// После прототипирования концепта персонаж изучает результат и уже точно знает рецепт
    /// изготовления предмета.
    /// </remarks>
    public class Concept : PropBase
    {

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="scheme"> Схема концепта. Сейчас всегда одна схема - conceptual-scheme. </param>
        /// <param name="prop"> Схема предмета, описанного в концепте. </param>
        [ExcludeFromCodeCoverage]
        public Concept(PropScheme scheme, PropScheme prop) : base(scheme)
        {
            Prop = prop;
        }

        /// <summary>
        /// Предмет, описанный в концептуальной схеме.
        /// </summary>
        public PropScheme Prop { get; }
    }
}
