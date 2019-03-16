using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Commands.Globe;
using Zilon.Core.Tactics;

public class GlobeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();

        Container.Bind<IGlobeUiState>().To<GlobeUiState>().AsSingle();

        Container.Bind<MoveGroupCommand>().AsSingle()
            .OnInstantiated<MoveGroupCommand>((c, i) =>
            {
                var scoreManager = Container.Resolve<IScoreManager>();
                if (scoreManager != null)
                {
                    i.ScoreManager = scoreManager;
                }

            }); ;
    }
}