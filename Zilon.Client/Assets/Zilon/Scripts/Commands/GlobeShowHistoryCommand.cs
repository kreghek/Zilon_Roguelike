using Zenject;

using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    /// <summary>
    /// Команда для отображения истории из глобальной карты.
    /// </summary>
    /// <remarks>
    /// Сделаны отдельно команды для сектора и для глобальной карты,
    /// потому что у них рависимости от похожих, но разных сервисов.
    /// 
    /// Эта команда испольнуется, когда история отображается первый раз при генерации мира.
    /// Потенциально, будет использоваться отдельным контроллом (но для этого в CanExecute
    /// нужно добавить проверку наличия книги истории в инвентаре).
    /// </remarks>
    internal sealed class GlobeShowHistoryCommand : ICommand
    {
        [Inject]
        private IGlobeModalManager _globeModalManager;

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            _globeModalManager.ShowHistoryBookModal();
        }
    }
}
