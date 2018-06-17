using Assets.Zilon.Scripts.Models.SectorScene;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Services.CombatMap;
using Zilon.Core.Services.MapGenerators;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();
        Container.Bind<ISectorManager>().To<SectorManager>().AsSingle();
        Container.Bind<IEventManager>().To<EventManager>().AsSingle();
        Container.Bind<IMapGenerator>().To<GridMapGenerator>().AsSingle();
        Container.Bind<IPlayerState>().To<PlayerState>().AsSingle();
    }
}