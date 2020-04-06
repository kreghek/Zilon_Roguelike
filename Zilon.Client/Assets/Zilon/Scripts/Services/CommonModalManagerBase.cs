using Zilon.Core.Client.Windows;
using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.Services
{
    public class CommonModalManagerBase : ModalManagerBase, ICommonModalManager
    {
        public QuitModalBody QuitModalPrefab;

        public ScoreModalBody ScoreModalPrefab;

        public PersonCreateModalBody PersonCreateModalPrefab;

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

        public void ShowCreatePersonModal(HumanPerson playerPerson)
        {
            var modalBody = CreateWindowHandler<PersonCreateModalBody>(PersonCreateModalPrefab.gameObject);
            modalBody.Init(playerPerson);
        }

        private void ShowQuitComfirmationModalInner(string caption, bool closeGame)
        {
            var modalBody = CreateWindowHandler<QuitModalBody>(QuitModalPrefab.gameObject);
            modalBody.Init(caption, closeGame);
        }
    }
}
