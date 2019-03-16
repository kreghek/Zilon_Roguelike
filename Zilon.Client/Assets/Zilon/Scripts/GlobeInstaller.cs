using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands.Globe;

public class GlobeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGlobeUiState>().To<GlobeUiState>().AsSingle();

        Container.Bind<MoveGroupCommand>().AsSingle();
    }
}