using Assets.Zilon.Scripts.Models.Modals;

using Zilon.Core.Client.Windows;
using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.Services
{
    public class CommonModalManagerBase : ModalManagerBase, ICommonModalManager
    {
        public QuitModalBody QuitModalPrefab;

        public ScoreModalBody ScoreModalPrefab;

        public void ShowQuitComfirmationModal()
        {
            ShowQuitComfirmationModalInner(QuitModalBehaviour.QuitGame);
        }

        public void ShowQuitTitleComfirmationModal()
        {
            ShowQuitComfirmationModalInner(QuitModalBehaviour.QuitToTitleMenu);
        }

        public void ShowScoreModal()
        {
            var modalBody = CreateWindowHandler<ScoreModalBody>(ScoreModalPrefab.gameObject);
            modalBody.Init();
        }

        private void ShowQuitComfirmationModalInner(QuitModalBehaviour quitModalBehaviour)
        {
            var modalBody = CreateWindowHandler<QuitModalBody>(QuitModalPrefab.gameObject);
            modalBody.Init(quitModalBehaviour);
        }
    }
}
