using Zenject;

using Zilon.Core.Commands.Globe;

public class GlobeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<MoveGroupCommand>().AsSingle();
    }
}