using System;
using System.Linq;
using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Commands
{
    //TODO Добавить тесты
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    public class AttackCommand : ActorCommandBase
    {
        public AttackCommand(ISectorManager sectorManager,
            IPlayerState playerState) :
            base(sectorManager, playerState)
        {
        }

        public override bool CanExecute()
        {
            var selectedActorViewModel = _playerState.SelectedActor;
            var targetNode = selectedActorViewModel.Actor.Node;

            var currentNode = _playerState.ActiveActor.Actor.Node;

            var targetHexNode = (HexNode)targetNode;
            var currentHexNode = (HexNode)currentNode;

            var line = CubeCoordsHelper.CubeDrawLine(currentHexNode.CubeCoords, targetHexNode.CubeCoords);

            for (var i = 1; i < line.Length; i++)
            {
                var prevPoint = line[i - 1];
                var testPoint = line[i];

                var prevNode = _sectorManager.CurrentSector.Map.Nodes
                    .SingleOrDefault(x => ((HexNode)x).CubeCoords == prevPoint);

                var testNode = _sectorManager.CurrentSector.Map.Nodes
                    .SingleOrDefault(x => ((HexNode)x).CubeCoords == testPoint);

                var prevEdges = from edge in _sectorManager.CurrentSector.Map.Edges
                                where edge.Nodes.Contains(prevNode)
                                select edge;

                var connectedEdge = (from edge in prevEdges
                                     where edge.Nodes.Contains(testNode)
                                     select edge).SingleOrDefault();

                if (connectedEdge == null)
                {
                    return false;
                }
            }

            //TODO Здесь должна быть проверка:
            // 1. Выбран ли вражеский юнит.
            // 2. Находится ли в пределах досягаемости оружия.
            // 3. Видим ли текущим актёром.
            // 4. Способно ли оружие атаковать.
            // 5. Доступен ли целевой актёр для атаки.
            // 6. Возможно ли выполнение каких-либо команд над актёрами
            // (Нельзя, если ещё выполняется текущая команда. Например, анимация перемещения.)
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            if (!CanExecute())
            {
                throw new InvalidOperationException("Не возможно выполнение команды.");
            }

            var sector = _sectorManager.CurrentSector;
            var selectedActorVM = _playerState.SelectedActor;

            var targetActor = selectedActorVM.Actor;
            _playerState.TaskSource.IntentAttack(targetActor);
            sector.Update();
        }
    }
}