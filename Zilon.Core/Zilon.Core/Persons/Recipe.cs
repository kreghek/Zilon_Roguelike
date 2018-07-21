using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    //TODO переименовать в концепт
    /// <summary>
    /// Рецепт крафта.
    /// </summary>
    /// <remarks>
    /// Является предметом, который можно изучить любым персонажем,
    /// если он соответствует условиям рецепта.
    /// 
    /// Концепт (рецепт) является концептуальной схемой предмета (черновик, чертёж).
    /// После прототипирования концепта персонаж изучает результат и уже точно знает рецепт
    /// изготовления предмета.
    /// </remarks>
    public class Recipe : PropBase
    {

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="scheme"> Схема рецепта. </param>
        public Recipe(PropScheme scheme, PropScheme prop) : base(scheme)
        {
            Prop = prop;
        }

        /// <summary>
        /// Предмет, описанный в концептуальной схеме.
        /// </summary>
        public PropScheme Prop { get; set; }
    }
}
