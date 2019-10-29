using Zilon.Core.Client.Windows;

namespace Assets.Zilon.Scripts.Services
{
    public class CommonModalManagerBase : ModalManagerBase, ICommonModalManager
    {
        public QuitModalBody QuitModalPrefab;

        public ScoreModalBody ScoreModalPrefab;

        public HistoryModalBody HistoryModalPrefab;

        public void ShowQuitComfirmationModal()
        {
            ShowQuitComfirmationModalInner("Quit game?", closeGame: true);
        }

        public void ShowQuitTitleComfirmationModal()
        {
            ShowQuitComfirmationModalInner("End game session?", closeGame: false);
        }

        public void ShowScoreModal()
        {
            var modalBody = CreateWindowHandler<ScoreModalBody>(ScoreModalPrefab.gameObject);
            modalBody.Init();
        }

        public void ShowHistoryBookModal()
        {
            var modalBody = CreateWindowHandler<HistoryModalBody>(HistoryModalPrefab.gameObject);
            modalBody.Init();
        }

        private void ShowQuitComfirmationModalInner(string caption, bool closeGame)
        {
            var modalBody = CreateWindowHandler<QuitModalBody>(QuitModalPrefab.gameObject);
            modalBody.Init(caption, closeGame);
        }
    }
}
