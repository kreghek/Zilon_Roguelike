using Zilon.Core.Tactics;

namespace Zilon.Core.Client
{
    /// <summary>
    /// Интерфейс модели представления актёра для клиенской части.
    /// </summary>
    public interface IActorViewModel : ISelectableViewModel
    {
        /// <summary>
        /// Актёр, который лежит в основе
        /// </summary>
        IActor Actor { get; set; }
    }
}