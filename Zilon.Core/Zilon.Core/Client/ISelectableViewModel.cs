namespace Zilon.Core.Client
{
    /// <summary>
    ///     Интерфейс моделей игрового мира, которые пользователь может выбрать.
    /// </summary>
    public interface ISelectableViewModel
    {
        object Item { get; }
    }
}