using Assets.Zilon.Scripts.Commands;
using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;
using Zilon.Core.Commands.Globe;
using Zilon.Core.Scoring;

public class GlobeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGlobeUiState>().To<GlobeUiState>().AsSingle();

        Container.Bind<MoveGroupCommand>().AsSingle()
            .OnInstantiated<MoveGroupCommand>((c, i) =>
            {
                //TODO Возможно, лучше будет извлекать значение из параметра "c".
                var scoreManager = Container.Resolve<IScoreManager>();
                if (scoreManager != null)
                {
                    i.ScoreManager = scoreManager;
                }

            });

        Container.Bind<IGlobeModalManager>().FromInstance(GetGlobeModalManager()).AsSingle();
        Container.Bind<ICommand>().WithId("show-history-command").To<GlobeShowHistoryCommand>().AsSingle();
    }

    private GlobeModalManager GetGlobeModalManager()
    {
        var modalManager = FindObjectOfType<GlobeModalManager>();
        return modalManager;
    }
}