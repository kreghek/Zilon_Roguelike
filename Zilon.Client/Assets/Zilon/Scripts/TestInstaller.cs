using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Services.CombatScene;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Services.CombatMap;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICombatService>().To<CombatService>().AsSingle();
        Container.Bind<ICommandManager>().To<QueueCommandManager>().AsSingle();
        Container.Bind<ICombatManager>().To<CombatManager>().AsSingle();
        Container.Bind<IPersonCommandHandler>().To<PersonCommandHandler>().AsSingle();
        Container.Bind<IEventManager>().To<EventManager>().AsSingle();
        Container.Bind<ICombatCommandResolver>().To<CombatCommandResolver>().AsSingle();
        Container.Bind<IMapGenerator>().To<GridMapGenerator>().AsSingle();
        Container.Bind<ICombatPlayerState>().To<CombatPlayerState>().AsSingle();
    }
}