using System;

using LightInject;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Emulation.Common
{
    public abstract class InitialzationBase
    {
        public int? DiceSeed { get; set; }

        protected InitialzationBase()
        { 
        }

        protected InitialzationBase(int diceSeed)
        {
            DiceSeed = diceSeed;
        }

        public virtual void RegisterServices(IServiceRegistry serviceRegistry)
        {
            RegisterSchemeService(serviceRegistry);
            RegisterAuxServices(serviceRegistry);
            RegisterPlayerServices(serviceRegistry);

            RegisterSectorServices(serviceRegistry);
        }

        public abstract void ConfigureAux(IServiceFactory serviceFactory);

        private void RegisterSectorServices(IServiceRegistry serviceRegistry)
        {
            RegisterClientServices(serviceRegistry);
            RegisterGameLoop(serviceRegistry);
            RegisterScopedSectorService(serviceRegistry);
            RegisterBot(serviceRegistry);
        }

        private void RegisterSchemeService(IServiceRegistry container)
        {
            container.Register<ISchemeLocator>(factory =>
            {
                //TODO Организовать отдельный общий метод/класс/фабрику для конструирования локатора схем.
                // Подобные конструкции распределены по всему проекту: в тестах, бенчах, окружении ботов.
                // Следует их объединить в одном месте.
                var schemePath = Environment.GetEnvironmentVariable("ZILON_LIV_SCHEME_CATALOG");

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            }, new PerContainerLifetime());

            container.Register<ISchemeService, SchemeService>(new PerContainerLifetime());

            container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(new PerContainerLifetime());
        }

        private void RegisterScopedSectorService(IServiceRegistry container)
        {
            //TODO сделать генераторы независимыми от сектора.
            // Такое время жизни, потому что в зависимостях есть менеджеры.
            container.Register<ISectorGenerator, SectorGenerator>(new PerScopeLifetime());
            container.Register<IMonsterGenerator, MonsterGenerator>(new PerScopeLifetime());
            container.Register<IChestGenerator, ChestGenerator>(new PerScopeLifetime());
            container.Register<ICitizenGenerator, CitizenGenerator>(new PerScopeLifetime());
            container.Register<ISectorFactory, SectorFactory>(new PerScopeLifetime());
            container.Register<ISectorManager, InfiniteSectorManager>(new PerScopeLifetime());
            container.Register<IActorManager, ActorManager>(new PerScopeLifetime());
            container.Register<IPropContainerManager, PropContainerManager>(new PerScopeLifetime());
            container.Register<ITacticalActUsageService, TacticalActUsageService>(new PerScopeLifetime());
            container.Register<IActorTaskSource, MonsterBotActorTaskSource>("monster", new PerScopeLifetime());
        }

        private void RegisterGameLoop(IServiceRegistry container)
        {
            container.Register<IGameLoop, GameLoop>(new PerScopeLifetime());
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices(IServiceRegistry container)
        {
            var dice = CreateRandomSeedAndDice();
            container.Register<IDice>(factory => dice, new PerContainerLifetime());
            container.Register<IDecisionSource, DecisionSource>(new PerContainerLifetime());
            container.Register<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>(new PerContainerLifetime());
            container.Register<IPerkResolver, PerkResolver>(new PerContainerLifetime());
            container.Register<IPropFactory, PropFactory>(new PerContainerLifetime());
            container.Register<IDropResolver, DropResolver>(new PerContainerLifetime());
            container.Register<IDropResolverRandomSource, DropResolverRandomSource>(new PerContainerLifetime());
            container.Register<ISurvivalRandomSource, SurvivalRandomSource>(new PerContainerLifetime());
            container.Register<IEquipmentDurableService, EquipmentDurableService>(new PerContainerLifetime());
            container.Register<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>(new PerContainerLifetime());
            container.Register<IHumanPersonFactory, RandomHumanPersonFactory>(new PerContainerLifetime());

            container.Register<IMapFactorySelector, LightInjectSwitchMapfactorySelector>(new PerContainerLifetime());
            container.Register<IMapFactory, RoomMapFactory>("room", new PerContainerLifetime());
            container.Register<IRoomGenerator, RoomGenerator>(new PerContainerLifetime());
            container.Register<IRoomGeneratorRandomSource, RoomGeneratorRandomSource>(new PerContainerLifetime());
            container.Register<IMapFactory, CellularAutomatonMapFactory>("cellular-automaton", new PerContainerLifetime());
            container.Register<IInteriorObjectRandomSource, InteriorObjectRandomSource>();
            container.Register<IMonsterGeneratorRandomSource, MonsterGeneratorRandomSource>(new PerContainerLifetime());
            container.Register<IChestGeneratorRandomSource, ChestGeneratorRandomSource>(new PerContainerLifetime());
            container.Register<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>(new PerContainerLifetime());
        }

        /// <summary>
        /// Создаёт кость и фиксирует зерно рандома.
        /// Если Зерно рандома не задано, то оно выбирается случайно.
        /// </summary>
        /// <returns> Экземпляр кости на основе выбранного или указанного ерна рандома. </returns>
        private IDice CreateRandomSeedAndDice()
        {
            IDice dice;
            if (DiceSeed == null)
            {
                var diceSeedFact = new Random().Next(int.MaxValue);
                DiceSeed = diceSeedFact;
                dice = new Dice(diceSeedFact);
            }
            else
            {
                dice = new Dice(DiceSeed.Value);
            }

            return dice;
        }

        private void RegisterClientServices(IServiceRegistry container)
        {
            container.Register<ISectorUiState, SectorUiState>(new PerScopeLifetime());
            container.Register<IInventoryState, InventoryState>(new PerScopeLifetime());
        }

        private void RegisterPlayerServices(IServiceRegistry container)
        {
            container.Register<IScoreManager, ScoreManager>(new PerContainerLifetime());
            container.Register<HumanPlayer>(new PerContainerLifetime());
            container.Register<IBotPlayer, BotPlayer>(new PerContainerLifetime());
        }

        protected abstract void RegisterBot(IServiceRegistry container);
    }
}
