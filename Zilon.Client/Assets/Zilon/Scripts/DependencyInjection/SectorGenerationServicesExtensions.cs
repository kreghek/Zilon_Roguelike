using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.RoomStyle;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class SectorGenerationServicesExtensions
    {
        public static void RegisterGenerationServices(this DiContainer diContainer)
        {
            diContainer.Bind<ISectorGenerator>().To<SectorGenerator>().AsSingle();
            diContainer.Bind<IMapFactory>().WithId("room").To<RoomMapFactory>().AsSingle();
            diContainer.Bind<IMapFactory>().WithId("cave").To<CellularAutomatonMapFactory>().AsSingle();
            diContainer.Bind<IMapFactorySelector>().To<MapFactorySelector>().AsSingle();
            diContainer.Bind<IRoomGeneratorRandomSource>().FromMethod(context =>
            {
                var linearDice = context.Container.ResolveId<IDice>("linear");
                var roomSizeDice = context.Container.ResolveId<IDice>("exp");
                return new RoomGeneratorRandomSource(linearDice, roomSizeDice);
            }).AsSingle();
            diContainer.Bind<IInteriorObjectRandomSource>().To<InteriorObjectRandomSource>().AsSingle();
            diContainer.Bind<IRoomGenerator>().To<RoomGenerator>().AsSingle();
            diContainer.Bind<IChestGenerator>().To<ChestGenerator>().AsSingle();
            diContainer.Bind<IChestGeneratorRandomSource>().To<ChestGeneratorRandomSource>().AsSingle();
            diContainer.Bind<IMonsterGenerator>().To<MonsterGenerator>().AsSingle();
            diContainer.Bind<IMonsterGeneratorRandomSource>().To<MonsterGeneratorRandomSource>().AsSingle();
            diContainer.Bind<ISectorFactory>().To<SectorFactory>().AsSingle();

            diContainer.Bind<IStaticObstaclesGenerator>().To<StaticObstaclesGenerator>().AsSingle();
        }
    }
}
